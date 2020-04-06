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
            playerSocket.Send(new HardCodedMessage(9, m.Name));//Welcome message
            m.LocalMessage(MessageType.System, "Server Programmed by Magistrate.");

            playerSocket.Send(new ReligionPacket(m));//TODO: Fix with correct Values
            playerSocket.Send(new MobileInventory(m));
            playerSocket.Send(new MobileEquipment(m));//TODO: Fix with correct Values

            playerSocket.Send(new MobileSpellList(m));
            playerSocket.Send(new MobileSkillList(m));

            playerSocket.Send(new NumberPlayers());

            playerSocket.Send(new MobileName(m));
            World.Broadcast(new HardCodedMessage(12, m.Name));//Player xxx joined broadcast????

            playerSocket.Send(new MobileIncoming(m));

            m.SendEverything();

            playerSocket.Send(new Unknown51());
            playerSocket.Send(new HouseingPacket(m));
            playerSocket.Send(new TeleportPacket((byte)m.Map.MapID, m.X, m.Y, (byte)m.Z));
            //playerSocket.Send(new GMobileName(m));
            playerSocket.Send(new Unknown03(m));
            playerSocket.Send(new Unknown03_1(m));
            playerSocket.Send(new Unknown03_2(m));
            //0x56 Packet 
            playerSocket.Send(new WeatherPacket(0, 0, 0));

            playerSocket.Send(new HealthManaEnergyPacket(m));
            playerSocket.Send(new MobileStats(m));
            playerSocket.Send(new MobileLevel(m));
            playerSocket.Send(new MinMaxDamageDisplay(m));

            playerSocket.Send(new Brightness(m));

            playerSocket.Send(new Unknown55());

            int unreadEmail = 0;
            foreach (MailMessage mm in m.Mail)
                if (!mm.Read)
                    unreadEmail++;

            if (unreadEmail > 0)
                playerSocket.Send(new TextMessage(MessageType.System, String.Format("You have {0} unread mail messages.", unreadEmail)));
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
                e.PlayerSocket.Send(new CharacterLoginReply(ALRReason.CharInUse));
                e.PlayerSocket.Dispose();
            }
            else
            {
                if (!PlayerManager.ValidPassword(e.Name, e.Password))
                {
                	Console.WriteLine( "Login: {0}: Invalid Password", e.PlayerSocket);
                    e.PlayerSocket.Send( new CharacterLoginReply( ALRReason.CharInvPw ) );
                	return;
                }
                IMobile m = a.Mobiles[charSlot];

                // Check if anyone is using this Character
                if (m.Map != MapManager.Internal)
                {
                    Console.WriteLine("Login: {0}: Account in use", e.PlayerSocket);
                    e.PlayerSocket.Send(new CharacterLoginReply(ALRReason.CharInUse));
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

                    m.Map = m.SpawnLocation.Map;
                    m.SetLocation(m.SpawnLocation.Location, false);

                    e.PlayerSocket.Send(new CharacterLoginReply(m));

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
                e.PlayerSocket.Send(new CharacterCreateReply(ALRReason.CharExist));
                return;
            }

            e.PlayerSocket.Send(new CharacterCreateReply(e.RejectReason));

        }

    }
}