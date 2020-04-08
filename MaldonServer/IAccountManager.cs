using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public class DataManager 
    {
        const string dataBase = "Data/MaldonDatatest.sqlite";
        private static SQLiteConnection m_dbConnection;

        private static void CreateTable(DataTable dt)
        {
            string command = "create table if not exists " + dt.TableName + " (";
            string columns = "";
            foreach (DataColumn dc in dt.Columns)
            {
                string dataType = dc.DataType.Name;
                switch (dc.DataType.Name)
                {
                    case "Int32":
                    case "Int16":
                        dataType = "INTEGER";
                        break;
                    case "String":
                        if (dc.MaxLength >= 0)
                            dataType = "varchar(" + dc.MaxLength + ")";
                        else
                            dataType = "varchar";//default to 255 characters
                        break;
                }
                string nullPrim = dc.AllowDBNull ? "" : "NOT NULL";
                if (dt.PrimaryKey.Contains(dc))
                    nullPrim = "PRIMARY KEY";
                if (dc.AutoIncrement)
                    nullPrim += " AUTOINCREMENT";
                if (dc.Unique)
                    nullPrim += " UNIQUE";

                if (columns != "")
                    columns += ", ";
                columns += string.Format("{0} {1} {2}", dc.ColumnName, dataType, nullPrim);
            }
            command = command + columns + ")";
            Console.WriteLine(command + "\r\n");
            SQLiteCommand sqlCommand = new SQLiteCommand(command, m_dbConnection);
            sqlCommand.ExecuteNonQuery();
        }

        public static SQLiteDataAdapter UseDataAdapter(DataTable dt)
        {
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM Account", m_dbConnection);
            dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            dataAdapter.Fill(dt);

            dataAdapter.DeleteCommand = new SQLiteCommand("delete from Account where id = :id", m_dbConnection);
            dataAdapter.DeleteCommand.Parameters.Add("id", DbType.Int32, 1, "id");

            dataAdapter.InsertCommand = new SQLiteCommand("INSERT INTO Account (Name, Password, Email, AccessLevel) " +
              "VALUES (:Name, :Password, :Email, :AccessLevel)", m_dbConnection);
            dataAdapter.InsertCommand.Parameters.Add("Name", DbType.String, 16, "Name");
            dataAdapter.InsertCommand.Parameters.Add("Password", DbType.String, 16, "Name");
            dataAdapter.InsertCommand.Parameters.Add("Email", DbType.String, 16, "Name");
            dataAdapter.InsertCommand.Parameters.Add("AccessLevel", DbType.String, 16, "AccessLevel");

            dataAdapter.UpdateCommand = new SQLiteCommand("Update Account set Name = :Name, Password = :Password, Email = :Email, AccessLevel = :AccessLevel " +
              "where id = :id", m_dbConnection);
            dataAdapter.UpdateCommand.Parameters.Add("Name", DbType.String, 16, "Name");
            dataAdapter.UpdateCommand.Parameters.Add("Password", DbType.String, 16, "Name");
            dataAdapter.UpdateCommand.Parameters.Add("Email", DbType.String, 16, "Name");
            dataAdapter.UpdateCommand.Parameters.Add("id", DbType.Int32, 256, "id");
            return dataAdapter;
        }

        //static DataManager()
        //{
        //    //if (!File.Exists(dataBase))
        //    SQLiteConnection.CreateFile(dataBase);

        //    m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", dataBase));
        //    m_dbConnection.Open();

        //    DataTable AccountTable = CreateAccountTable();
        //    //Create Table
        //    CreateTable(AccountTable);

        //    SQLiteDataAdapter myAdapter = UseDataAdapter(AccountTable);

        //    DataRow dr = AccountTable.NewRow();
        //    dr["Name"] = "NV";
        //    dr["Password"] = "NV";
        //    dr["Email"] = "NV";
        //    dr["AccessLevel"] = (int)AccessLevel.Administrator;
        //    AccountTable.Rows.Add(dr);

        //    //UpdateTable(AccountTable);
        //    myAdapter.Update(AccountTable);
        //    ShowTable(AccountTable);

        //    DataRow dr2 = AccountTable.NewRow();
        //    dr2["Name"] = "NV2";
        //    dr2["Password"] = "NV2";
        //    dr2["Email"] = "NV2";
        //    dr["AccessLevel"] = 0;
        //    AccountTable.Rows.Add(dr2);
        //    myAdapter.Update(AccountTable);
        //    ShowTable(AccountTable);

        //    //UpdateTable(AccountTable);
        //    dr["Name"] = "NewValueHere";
        //    //UpdateTable(AccountTable);
        //    myAdapter.Update(AccountTable);
        //    ShowTable(AccountTable);
        //    dr2.Delete();
        //    //UpdateTable(AccountTable);
        //    myAdapter.Update(AccountTable);
        //    ShowTable(AccountTable);

        //    string cs = "Data Source=:memory:";
        //    string stm = "SELECT SQLITE_VERSION()";

        //    SQLiteConnection con = new SQLiteConnection(cs);
        //    con.Open();

        //    var cmd = new SQLiteCommand(stm, con);
        //    string version = cmd.ExecuteScalar().ToString();

        //    Console.WriteLine("SQLite version: {0}", version);
        //}

        private static DataTable CreateAccountTable()
        {
            
            DataTable accountTable = new DataTable("Account");

            //primary Key column
            DataColumn colId = new DataColumn
            {
                AutoIncrement = true,
                AutoIncrementSeed = 1,
                DataType = typeof(Int32),
                ColumnName = "id",
                ReadOnly = true,
                Unique = true
            };
            accountTable.Columns.Add(colId);

            //stored columns
            accountTable.Columns.Add(new DataColumn { ColumnName = "Name", DataType = typeof(String), MaxLength=16, Unique=true, AllowDBNull=false });
            accountTable.Columns.Add(new DataColumn { ColumnName = "Password", DataType = typeof(String), MaxLength = 16, AllowDBNull = false });
            accountTable.Columns.Add(new DataColumn { ColumnName = "Email", DataType = typeof(String), MaxLength = 16, AllowDBNull = false });
            accountTable.Columns.Add(new DataColumn { ColumnName = "AccessLevel", DataType = typeof(Int32), AllowDBNull = false });

            // Set the OrderId column as the primary key.
            accountTable.PrimaryKey = new DataColumn[] { colId };

            return accountTable;
        }

        private static void ShowTable(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
            {
                Console.Write("{0,-14}", col.ColumnName);
            }
            Console.WriteLine();

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    if (col.DataType.Equals(typeof(DateTime)))
                        Console.Write("{0,-14:d}", row[col]);
                    else if (col.DataType.Equals(typeof(Decimal)))
                        Console.Write("{0,-14:C}", row[col]);
                    else
                        Console.Write("{0,-14}", row[col]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    public interface IAccountManager
    {
        void CreateAccount(PlayerSocket ps, string un, string pw, string em);
        void LoginAccount(PlayerSocket ps, string un, string pw);
    }
}
