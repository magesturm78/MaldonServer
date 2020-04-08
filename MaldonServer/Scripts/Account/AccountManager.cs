using System;
using System.Collections;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;
using MaldonServer.Scripts;

namespace MaldonServer.Scripts.Accounting
{
    public class AccountManager : IAccountManager
    {
        private Hashtable Accounts = new Hashtable();

        public Account GetAccount(string username)
        {
            return Accounts[username] as Account;
        }

        public Account AddAccount(string user, string pass, string email)
        {
            Account a = new Account(user, pass, email);
            if (Accounts.Count == 0)
                a.AccessLevel = AccessLevel.Administrator;

            Accounts[a.UserName] = a;

            return a;
        }

        public void LoginAccount(PlayerSocket ps, string un, string pw)
        {
            Account acct = GetAccount(un);
            if (acct == null)
            {
                Console.WriteLine("Login: {0}: Invalid username '{1}'", ps, un);
                ps.Send(new AccountCreateLoginReplyPacket(ALRReason.InvalidAccount));
            }
            else if (acct.Banned)
            {
                Console.WriteLine("Login: {0}: Banned account '{1}'", ps, un);
                ps.Send(new AccountCreateLoginReplyPacket(ALRReason.Blocked));
            }
            else if (!acct.ValidPassword(pw))
            {
                Console.WriteLine("Login: {0}: Invalid password for '{1}'", ps, un);
                ps.Send(new AccountCreateLoginReplyPacket(ALRReason.BadPass));
            }
            else
            {
                Console.WriteLine("Login: {0}: Valid credentials for '{1}'", ps, un);
                ps.Account = acct;
                acct.PlayerSocket = ps;
                ps.Send(new AccountCreateLoginReplyPacket(ALRReason.CorrectPassword));
            }
        }

        public void CreateAccount(PlayerSocket ps, string un, string pw, string em)
        {
            Account acct = GetAccount(un);
            if (acct == null)
            {
                if (string.IsNullOrEmpty(un) || string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(em))
                    return;

                bool isSafe = true;

                for (int i = 0; isSafe && i < un.Length; ++i)
                    isSafe = (un[i] >= 0x20 && un[i] < 0x80);

                for (int i = 0; isSafe && i < pw.Length; ++i)
                    isSafe = (pw[i] >= 0x20 && pw[i] < 0x80);

                for (int i = 0; isSafe && i < em.Length; ++i)
                    isSafe = (em[i] >= 0x20 && em[i] < 0x80);

                if (isSafe)
                {
                    Console.WriteLine("Login: {0}: Creating new account '{1}'", ps, un);
                    acct = AddAccount(un, pw, em);
                }

                if (acct == null)
                    ps.Send(new AccountCreateLoginReplyPacket(ALRReason.InvalidAccount));
                else
                    ps.Send(new AccountCreateLoginReplyPacket(ALRReason.AccountCreated));
            }
            else
            {
                ps.Send(new AccountCreateLoginReplyPacket(ALRReason.AccountExists));
            }
        }

        private void DoLogin(PlayerSocket playerSocket)
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

        //public void EventSink_CharacterLogin(CharacterLoginEventArgs e)
        //{
        //    int charSlot = 10;
        //    IAccount a = e.PlayerSocket.Account;

        //    for (int i = 0; i < a.Mobiles.Length; ++i)
        //    {
        //        IMobile check = a.Mobiles[i];
        //        if (check != null && check.Name == e.Name)
        //        {
        //            charSlot = i;
        //        }
        //    }
        //    if (a == null || charSlot < 0 || charSlot >= a.Mobiles.Length)
        //    {
        //        Console.WriteLine("Login: {0}: Character not assigned to account", e.PlayerSocket);
        //        e.PlayerSocket.Send(new CharacterLoginReplyPacket(ALRReason.CharInUse));
        //        e.PlayerSocket.Dispose();
        //    }
        //    else
        //    {
        //        if (!PlayerManager.ValidPassword(e.Name, e.Password))
        //        {
        //            Console.WriteLine("Login: {0}: Invalid Password", e.PlayerSocket);
        //            e.PlayerSocket.Send(new CharacterLoginReplyPacket(ALRReason.CharInvPw));
        //            return;
        //        }
        //        PlayerMobile m = a.Mobiles[charSlot] as PlayerMobile;

        //        // Check if anyone is using this Character
        //        if (m.Map != MapManager.Internal)
        //        {
        //            Console.WriteLine("Login: {0}: Account in use", e.PlayerSocket);
        //            e.PlayerSocket.Send(new CharacterLoginReplyPacket(ALRReason.CharInUse));
        //            return;
        //        }

        //        if (m == null)
        //        {
        //            e.PlayerSocket.Dispose();
        //        }
        //        else
        //        {
        //            if (m.PlayerSocket != null)
        //                m.PlayerSocket.Dispose();

        //            e.PlayerSocket.Mobile = m;
        //            m.PlayerSocket = e.PlayerSocket;

        //            m.Spawn();

        //            e.PlayerSocket.Send(new CharacterLoginReplyPacket(m));

        //            DoLogin(e.PlayerSocket);
        //        }
        //    }

        //}

        //public void EventSink_CharacterCreate(CharacterCreateEventArgs e)
        //{
        //    IAccount a = e.PlayerSocket.Account;

        //    if (PlayerManager.CharacterExists(e.Name))
        //    {
        //        Console.WriteLine("Login: {0}: Character name {1} already exists.", e.PlayerSocket, e.Name);
        //        e.PlayerSocket.Send(new CharacterCreateReplyPacket(ALRReason.CharExist));
        //        return;
        //    }

        //    e.PlayerSocket.Send(new CharacterCreateReplyPacket(e.RejectReason));

        //}
    }
}