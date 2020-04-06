using System;
using System.Collections;

namespace MaldonServer.Scripts.Accounting
{
    public class AccountManager
    {
        private static Hashtable Accounts = new Hashtable();

        public static Account GetAccount(string username)
        {
            return Accounts[username] as Account;
        }

        public static Account AddAccount(string user, string pass)
        {
            Account a = new Account(user, pass);
            if (Accounts.Count == 0)
                a.AccessLevel = AccessLevel.Administrator;

            Accounts[a.UserName] = a;

            return a;
        }
    }
}