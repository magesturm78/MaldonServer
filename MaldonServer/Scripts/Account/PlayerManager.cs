using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using MaldonServer;
using MaldonServer.Network;

namespace MaldonServer.Scripts.Accounting
{
	public class PlayerManager
	{
        public static void Initialize()
        {
            //CreateData();
        }

        private static void CreateData()
        {
            SQLiteCommand dropCommand = new SQLiteCommand("DROP Table IF EXISTS Player", DataManager.Connection);
            dropCommand.ExecuteNonQuery();

            //create table
            string sql = "create table IF NOT EXISTS Player " +
                "(id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, " +
                "account_id INTEGER NOT NULL, " +
                "Name varchar(16) NOT NULL UNIQUE, " +
                "Password varchar(16) NOT NULL, " +
                "Level INTEGER default 1, " +
                "Experience INTEGER default 0, " +
                "Gender INTEGER default 0, " +
                "hair INTEGER default 0, " +
                "spawn_map INTEGER default 7, " +
                "spawn_x INTEGER default 343, " +
                "spawn_y INTEGER default 91, " +
                "spawn_z INTEGER default 0, " +
                "available_points INTEGER default 0, " +
                "strength INTEGER default 20, " +
                "defence INTEGER default 20, " +
                "constitution INTEGER default 20, " +
                "intelligence INTEGER default 20, " +
                "magic INTEGER default 20, " +
                "stamina INTEGER default 20, " +
                "religion INTEGER default 0, " +
                "FOREIGN KEY (account_id) REFERENCES account (id))";
            SQLiteCommand sqlCommand = new SQLiteCommand(sql, DataManager.Connection);
            sqlCommand.ExecuteNonQuery();

            //Create indexes of fields that are searched
            sqlCommand = new SQLiteCommand("CREATE UNIQUE INDEX idx_player_id ON player(id)", DataManager.Connection);
            sqlCommand.ExecuteNonQuery();

            sqlCommand = new SQLiteCommand("CREATE UNIQUE INDEX idx_player_account_id ON player(account_id)", DataManager.Connection);
            sqlCommand.ExecuteNonQuery();

            sqlCommand = new SQLiteCommand("CREATE UNIQUE INDEX idx_player_name ON player(name)", DataManager.Connection);
            sqlCommand.ExecuteNonQuery();
        }

        public static bool PlayerExists(string name)
		{
            Console.WriteLine("Check if player exisits");
            SQLiteCommand sqlCommand = new SQLiteCommand("Select Count(id) from Player where Name = :Name", DataManager.Connection);
            sqlCommand.Parameters.Add(new SQLiteParameter("Name", name));
            long count = (long)sqlCommand.ExecuteScalar();
            return (count > 0);
		}

        public static void CreatePlayer(int accountId, string name, string password, byte gender, byte hair)
        {
            Console.WriteLine("Create player");
            byte hairID = 0;
            switch (hair)
            {
                case 0://black
                    hairID = 61;
                    break;
                case 1://blond
                    hairID = 60;
                    break;
                case 2://blue
                    hairID = 63;
                    break;
                case 3://brown
                    hairID = 62;
                    break;
                case 4://green
                    hairID = 64;
                    break;
                case 5://red
                    hairID = 59;
                    break;
            }

            SQLiteCommand sqlCommand = new SQLiteCommand("Insert into Player (account_id, Name, Password, Gender, hair)" +
                " values (:account_id, :Name, :Password, :Gender, :hair)", DataManager.Connection);

            sqlCommand.Parameters.Add(new SQLiteParameter("account_id", accountId));
            sqlCommand.Parameters.Add(new SQLiteParameter("Name", name));
            sqlCommand.Parameters.Add(new SQLiteParameter("Password", password));
            sqlCommand.Parameters.Add(new SQLiteParameter("Gender", gender));
            sqlCommand.Parameters.Add(new SQLiteParameter("hair", hairID));
            sqlCommand.ExecuteNonQuery();

            //newChar.EquipItem(new Candle());
            //newChar.EquipItem(new Robe());
        }

        public static List<IMobile> GetPlayerList(int accountID)
        {
            SQLiteCommand sqlCommand = new SQLiteCommand("Select ID, Name, Password, Level, Experience, Gender, hair," +
                "spawn_map, spawn_x, spawn_y , spawn_z, available_points, strength, defence, constitution, intelligence, magic," +
                "stamina, religion from Player where account_id = :account_id", DataManager.Connection);
            sqlCommand.Parameters.Add(new SQLiteParameter("account_id", accountID));

            List<IMobile> players = new List<IMobile>();
            using (SQLiteDataReader dr = sqlCommand.ExecuteReader())
            {
                while (dr.Read())
                {
                    try
                    {
                        int index = 0;
                        int id = dr.GetInt32(index++);
                        string name = dr.GetString(index++);
                        string password = dr.GetString(index++);
                        int level = dr.GetInt32(index++);
                        int experience = dr.GetInt32(index++);
                        byte gender = (byte)dr.GetInt32(index++);
                        byte hair = (byte)dr.GetInt32(index++);
                        byte spawnMap = (byte)dr.GetInt32(index++);
                        int spawnX = dr.GetInt32(index++);
                        int spawnY = dr.GetInt32(index++);
                        byte spawnZ = (byte)dr.GetInt32(index++);
                        int availablePoints = dr.GetInt32(index++);
                        int strength = dr.GetInt32(index++);
                        int defence = dr.GetInt32(index++);
                        int consititution = dr.GetInt32(index++);
                        int intelligence = dr.GetInt32(index++);
                        int magic = dr.GetInt32(index++);
                        int stamina = dr.GetInt32(index++);
                        byte religion = (byte)dr.GetInt32(index++);

                        PlayerMobile pm = new PlayerMobile(id, name, password, level, experience, gender, hair, spawnMap, spawnX, spawnY, spawnZ,
                                                        availablePoints, strength, defence, consititution, intelligence, magic, stamina, religion);

                        players.Add((IMobile)pm);
                    } 
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            return players;
        }
    }
}