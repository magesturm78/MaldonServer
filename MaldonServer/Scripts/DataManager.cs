using System;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace MaldonServer.Scripts
{
    public class DataManager 
    {
        const string dataBase = "Data/MaldonData.sqlite";
        public static SQLiteConnection Connection { get; private set; }

        static DataManager()
        {
            if (!File.Exists(dataBase))
                SQLiteConnection.CreateFile(dataBase);

            Connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", dataBase));
            Connection.Open();

        }
    }
}
