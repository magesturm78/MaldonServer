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
        private static SQLiteDataAdapter dataAdapter;

        private static DataTable AccountsData;

        public static void Initialize()
        {
            World.SetAccountManager(new AccountManager());
            CreateAccountData();
        }

        private static void CreateAccountData()
        {
            string command = "create table if not exists Account" +
                "(id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, " +
                "Name varchar(16) NOT NULL UNIQUE, " +
                "Password varchar(16) NOT NULL, " + 
                "Email varchar(16) NOT NULL," +
                "AccessLevel INTEGER NOT NULL)";
            SQLiteCommand sqlCommand = new SQLiteCommand(command, DataManager.Connection);
            sqlCommand.ExecuteNonQuery();

            CreateDataAdapter();
        }

        public Account GetAccount(string username)
        {
            DataRow[] foundRows = AccountsData.Select("Name = '" + username + "'");
            if (foundRows.Length == 0) return null;
            if (foundRows.Length > 1) return null;//Should not happen
            DataRow dr = foundRows[0];
            string nm = dr["Name"].ToString();
            string pw = dr["Password"].ToString();
            string em = dr["Email"].ToString();
            AccessLevel level = (AccessLevel)Int32.Parse(dr["AccessLevel"].ToString());

            return new Account(nm,pw,em);
        }

        public Account AddAccount(string user, string pass, string email)
        {
            Account a = new Account(user, pass, email);
            if (AccountsData.Rows.Count == 0)
                a.AccessLevel = AccessLevel.Administrator;

            DataRow dr = AccountsData.NewRow();
            dr["Name"] = user;
            dr["Password"] = pass;
            dr["Email"] = email;
            dr["AccessLevel"] = (int)a.AccessLevel;

            AccountsData.Rows.Add(dr);
            dataAdapter.Update(AccountsData);

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

        private static void CreateDataAdapter()
        {
            dataAdapter = new SQLiteDataAdapter("SELECT * FROM Account", DataManager.Connection);
            dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            AccountsData = new DataTable("Accounts");
            dataAdapter.Fill(AccountsData);

            dataAdapter.DeleteCommand = new SQLiteCommand("delete from Account where id = :id", DataManager.Connection);
            dataAdapter.DeleteCommand.Parameters.Add("id", DbType.Int32, 1, "id");

            dataAdapter.InsertCommand = new SQLiteCommand("INSERT INTO Account (Name, Password, Email, AccessLevel) " +
              "VALUES (:Name, :Password, :Email, :AccessLevel)", DataManager.Connection);
            dataAdapter.InsertCommand.Parameters.Add("Name", DbType.String, 16, "Name");
            dataAdapter.InsertCommand.Parameters.Add("Password", DbType.String, 16, "Name");
            dataAdapter.InsertCommand.Parameters.Add("Email", DbType.String, 16, "Name");
            dataAdapter.InsertCommand.Parameters.Add("AccessLevel", DbType.String, 16, "AccessLevel");

            dataAdapter.UpdateCommand = new SQLiteCommand("Update Account set Name = :Name, Password = :Password, Email = :Email, AccessLevel = :AccessLevel " +
              "where id = :id", DataManager.Connection);
            dataAdapter.UpdateCommand.Parameters.Add("Name", DbType.String, 16, "Name");
            dataAdapter.UpdateCommand.Parameters.Add("Password", DbType.String, 16, "Name");
            dataAdapter.UpdateCommand.Parameters.Add("Email", DbType.String, 16, "Name");
            dataAdapter.UpdateCommand.Parameters.Add("id", DbType.Int32, 256, "id");
        }
    }
}