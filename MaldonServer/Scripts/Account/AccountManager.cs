using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Collections;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;
using MaldonServer.Scripts;

namespace MaldonServer.Scripts.Accounting
{
    public class AccountManager : IAccountManager
    {
        private static bool AdminAccountExists = false;

        public static void Initialize()
        {
            World.SetAccountManager(new AccountManager());

            SQLiteCommand sqlCommand = new SQLiteCommand("Select Count(id) from Account Where AccessLevel = :AccessLevel", DataManager.Connection);
            sqlCommand.Parameters.Add(new SQLiteParameter("AccessLevel", (int)AccessLevel.Administrator));
            long count = (long)sqlCommand.ExecuteScalar();
            AdminAccountExists = (count > 0);
        }

        private static void CreateData()
        {
            string command = "create table Account" +
                "(id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, " +
                "Name varchar(16) NOT NULL UNIQUE, " +
                "Password varchar(16) NOT NULL, " + 
                "Email varchar(16) NOT NULL," +
                "AccessLevel INTEGER NOT NULL," +
                "Banned INTEGER DEFAULT 0 NOT NULL)";//0 = false, 1 = true
            SQLiteCommand sqlCommand = new SQLiteCommand(command, DataManager.Connection);
            sqlCommand.ExecuteNonQuery();

            sqlCommand = new SQLiteCommand("CREATE UNIQUE INDEX idx_accound_id ON account(id)", DataManager.Connection);
            sqlCommand.ExecuteNonQuery();

            sqlCommand = new SQLiteCommand("CREATE UNIQUE INDEX idx_accound_name ON account(name)", DataManager.Connection);
            sqlCommand.ExecuteNonQuery();
        }

        private bool AccountExisits(string username)
        {
            SQLiteCommand sqlCommand = new SQLiteCommand("Select Count(id) from Account where Name = :Name", DataManager.Connection);
            sqlCommand.Parameters.Add(new SQLiteParameter("Name", username));
            long count = (long)sqlCommand.ExecuteScalar();
            return (count > 0);
        }

        private Account GetAccount(string username)
        {
            SQLiteCommand sqlCommand = new SQLiteCommand("Select id, Name, Password, AccessLevel, Banned from Account where Name = :Name", DataManager.Connection);
            sqlCommand.Parameters.Add(new SQLiteParameter("Name", username));
            Account acct = null;
            using (SQLiteDataReader dr = sqlCommand.ExecuteReader())
            {
                int index = 0;
                if (dr.Read())
                {
                    int id = dr.GetInt32(index++);
                    string name = dr.GetString(index++);
                    string password = dr.GetString(index++);
                    AccessLevel al = (AccessLevel)dr.GetInt32(index++);
                    bool banned = dr.GetInt32(index++) == 1;//0 = false, 1 = true
                    acct = new Account(id, name, password, al, banned);
                }
            }
            return acct;
        }

        private bool AddAccount(string user, string pass, string email)
        {
            AccessLevel al = AccessLevel.Peasant;
            if (!AdminAccountExists)
            {
                al = AccessLevel.Administrator;
                AdminAccountExists = true;
            }

            SQLiteCommand sqlCommand = new SQLiteCommand("INSERT INTO Account (Name, Password, Email, AccessLevel) " +
                "VALUES (:Name, :Password, :Email, :AccessLevel)", DataManager.Connection);
            sqlCommand.Parameters.Add(new SQLiteParameter("Name", user));
            sqlCommand.Parameters.Add(new SQLiteParameter("Password", pass));
            sqlCommand.Parameters.Add(new SQLiteParameter("Email", email));
            sqlCommand.Parameters.Add(new SQLiteParameter("AccessLevel", ((int)al)));
            sqlCommand.ExecuteNonQuery();

            return true;
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
                ps.Send(new AccountCreateLoginReplyPacket(ALRReason.InvalidAccount));
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
            if (!AccountExisits(un))
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

                bool acctAdded = false; 
                if (isSafe)
                {
                    Console.WriteLine("Login: {0}: Creating new account '{1}'", ps, un);
                    acctAdded = AddAccount(un, pw, em);
                }

                Console.WriteLine("Login: {0}: Created new account {1}", ps, acctAdded);
                if (acctAdded)
                    ps.Send(new AccountCreateLoginReplyPacket(ALRReason.AccountCreated));
                else
                    ps.Send(new AccountCreateLoginReplyPacket(ALRReason.InvalidAccount));
            }
            else
            {
                ps.Send(new AccountCreateLoginReplyPacket(ALRReason.AccountExists));
            }
        }

        public void LostPassword(PlayerSocket ps, string accountName)
        {
            Console.WriteLine("Login: {0}: Lost Password for account {1}", ps, accountName);
        }
    }
}