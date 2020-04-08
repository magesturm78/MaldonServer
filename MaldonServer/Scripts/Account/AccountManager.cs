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

        public static void Initialize()
        {
            World.SetAccountManager(new AccountManager());
        }

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

    }
}