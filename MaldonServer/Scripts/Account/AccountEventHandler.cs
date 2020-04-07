using System;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;
using MaldonServer.Scripts;

namespace MaldonServer.Scripts.Accounting
{
    public class AccountEventHandler
    {
        public static void Initialize()
        {
            EventSink.AccountLogin += new AccountLoginEventHandler(EventSink_AccountLogin);
            EventSink.AccountCreate += new AccountCreateEventHandler(EventSink_AccountCreate);
            EventSink.CharacterLogin += new CharacterLoginEventHandler(EventSink_CharacterLogin);
            EventSink.CharacterCreate += new CharacterCreateEventHandler(EventSink_CharacterCreate);
        }

        private static Account CreateAccount(PlayerSocket playerSocket, string un, string pw)
        {
            if (un.Length == 0 || pw.Length == 0)
                return null;

            bool isSafe = true;

            for (int i = 0; isSafe && i < un.Length; ++i)
                isSafe = (un[i] >= 0x20 && un[i] < 0x80);

            for (int i = 0; isSafe && i < pw.Length; ++i)
                isSafe = (pw[i] >= 0x20 && pw[i] < 0x80);

            if (!isSafe)
                return null;

            Console.WriteLine("Login: {0}: Creating new account '{1}'", playerSocket, un);
            return AccountManager.AddAccount(un, pw);
        }

        public static void EventSink_AccountLogin(AccountLoginEventArgs e)
        {
            string un = e.Username;
            string pw = e.Password;

            e.Accepted = false;
            Account acct = AccountManager.GetAccount(un);
            if (acct == null)
            {
                Console.WriteLine("Login: {0}: Invalid username '{1}'", e.PlayerSocket, un);
                e.Reply = ALRReason.InvalidAccount;
            }
            else if (acct.Banned)
            {
                Console.WriteLine("Login: {0}: Banned account '{1}'", e.PlayerSocket, un);
                e.Reply = ALRReason.Blocked;
            }
            else if (!acct.ValidPassword(pw))
            {
                Console.WriteLine("Login: {0}: Invalid password for '{1}'", e.PlayerSocket, un);
                e.Reply = ALRReason.BadPass;
            }
            else
            {
                Console.WriteLine( "Login: {0}: Valid credentials for '{1}'", e.PlayerSocket, un );
                e.PlayerSocket.Account = acct;
                e.Accepted = true;
                e.Reply = ALRReason.CorrectPassword;
            }
        }

        public static void EventSink_AccountCreate(AccountCreateEventArgs e)
        {
            string un = e.Username;
            string pw = e.Password;
            string em = e.Email;

            e.Accepted = false;
            Account acct = AccountManager.GetAccount(un);
            if (acct == null)
            {
                e.PlayerSocket.Account = acct = CreateAccount(e.PlayerSocket, un, pw);
                e.Accepted = acct == null ? false : true;

                if (!e.Accepted)
                    e.Reply = ALRReason.InvalidAccount;//not sure what to pass here...
                else
                {
                    e.Reply = ALRReason.AccountCreated;
                }
            }
            else
            {
                e.Reply = ALRReason.AccountExists;
            }

            //if (!e.Accepted)
            //    AccountAttackLimiter.RegisterInvalidAccess(e.State);

        }

        private static void DoLogin(PlayerSocket playerSocket)
        {
            IMobile m = playerSocket.Mobile;
            playerSocket.Send(new HardCodedMessagePacket(9, m.Name));//Welcome message
            m.LocalMessage(MessageType.System, "Server Programmed by Magistrate.");

            playerSocket.Send(new ReligionPacket(m));//TODO: Fix with correct Values
            playerSocket.Send(new PlayerInventoryPacket(m));
            playerSocket.Send(new PlayerEquipmentPacket(m));//TODO: Fix with correct Values

            playerSocket.Send(new PlayerSpellListPacket(m));
            playerSocket.Send(new PlayerSkillListPacket(m));

            playerSocket.Send(new NumberPlayersPacket());

            playerSocket.Send(new PlayerNamePacket(m));
            World.Broadcast(new HardCodedMessagePacket(12, m.Name));//Player xxx joined broadcast????

            playerSocket.Send(new PlayerIncomingPacket(m));

            m.SendEverything();

            playerSocket.Send(new Unk51Packet());
            playerSocket.Send(new PlayerHouseingPacket(m));
            playerSocket.Send(new PlayerTeleportPacket((byte)m.Map.MapID, m.X, m.Y, (byte)m.Z));
            //playerSocket.Send(new GMobileName(m));
            playerSocket.Send(new Unk03Packet(m));
            playerSocket.Send(new Unk03_1Packet(m));
            playerSocket.Send(new PlayerLocationPacket(m));
            //0x56 Packet 
            playerSocket.Send(new WeatherPacket(0, 0, 0));

            playerSocket.Send(new PlayerHMEPacket(m));
            playerSocket.Send(new PlayerStatsPacket(m));
            playerSocket.Send(new PlayerLevelPacket(m));
            playerSocket.Send(new PlayerMinMaxDamagePacket(m));

            playerSocket.Send(new BrightnessPacket(m.Map));

            playerSocket.Send(new Unk55Packet());

            int unreadEmail = 0;
            foreach (MailMessage mm in m.Mail)
                if (!mm.Read)
                    unreadEmail++;

            if (unreadEmail > 0)
                playerSocket.Send(new TextMessagePacket(MessageType.System, String.Format("You have {0} unread mail messages.", unreadEmail)));
        }

        public static void EventSink_CharacterLogin(CharacterLoginEventArgs e)
        {
            int charSlot = 10;
            IAccount a = e.PlayerSocket.Account;

            for (int i = 0; i < a.Mobiles.Length; ++i)
            {
                IMobile check = a.Mobiles[i];
                if (check != null && check.Name == e.Name)
                {
                    charSlot = i;
                }
            }
            if (a == null || charSlot < 0 || charSlot >= a.Mobiles.Length)
            {
                Console.WriteLine("Login: {0}: Character not assigned to account", e.PlayerSocket);
                e.PlayerSocket.Send(new CharacterLoginReplyPacket(ALRReason.CharInUse));
                e.PlayerSocket.Dispose();
            }
            else
            {
                if (!PlayerManager.ValidPassword(e.Name, e.Password))
                {
                	Console.WriteLine( "Login: {0}: Invalid Password", e.PlayerSocket);
                    e.PlayerSocket.Send( new CharacterLoginReplyPacket( ALRReason.CharInvPw ) );
                	return;
                }
                PlayerMobile m = a.Mobiles[charSlot] as PlayerMobile;

                // Check if anyone is using this Character
                if (m.Map != MapManager.Internal)
                {
                    Console.WriteLine("Login: {0}: Account in use", e.PlayerSocket);
                    e.PlayerSocket.Send(new CharacterLoginReplyPacket(ALRReason.CharInUse));
                    return;
                }

                if (m == null)
                {
                    e.PlayerSocket.Dispose();
                }
                else
                {
                    if (m.PlayerSocket != null)
                        m.PlayerSocket.Dispose();

                    e.PlayerSocket.Mobile = m;
                    m.PlayerSocket = e.PlayerSocket;

                    m.Spawn();

                    e.PlayerSocket.Send(new CharacterLoginReplyPacket(m));

                    DoLogin(e.PlayerSocket);
                }
            }

        }

        public static void EventSink_CharacterCreate(CharacterCreateEventArgs e)
        {
            IAccount a = e.PlayerSocket.Account;

            if (CharacterManager.CharacterExists(e.Name))
            {
                Console.WriteLine("Login: {0}: Character name {1} already exists.", e.PlayerSocket, e.Name);
                e.PlayerSocket.Send(new CharacterCreateReplyPacket(ALRReason.CharExist));
                return;
            }

            e.PlayerSocket.Send(new CharacterCreateReplyPacket(e.RejectReason));

        }

    }
}