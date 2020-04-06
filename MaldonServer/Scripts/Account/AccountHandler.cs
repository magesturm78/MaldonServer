using System;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Accounting
{
    public class AccountHandler
    {
        public static void Initialize()
        {
            EventSink.AccountLogin += new AccountLoginEventHandler(EventSink_AccountLogin);
            EventSink.AccountCreate += new AccountCreateEventHandler(EventSink_AccountCreate);
            //EventSink.GameLogin += new GameLoginEventHandler(EventSink_GameLogin);
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
            //Account acct = Accounts.GetAccount(un);
            Console.WriteLine("EventSink_AccountLogin");
            e.Reply = ALRReason.BadPass;
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

    }
}