using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
    public struct ItemSpawn
    {
        public Int16 X;
        public Int16 Y;
        public byte Z;
        public Int16 ItemID;
        public byte LocationID;
        public bool CanPickup;
        public bool Spawned;
    }

    public class Map: IMap
    {
        const bool DEBUG = false;
        const bool INFO = false;

        struct MapData
        {
            public Int16 icon1;
            public byte icon1index;
            public Int16 icon2;
            public byte icon2index;
            public Int16 item1;
            public byte item1index;
            public Int16 item2;
            public byte item2index;
            public string MapCommand;
            public string Script;
            public DoorStatus DoorStatus;
        }

        private bool loaded;
        private MapData[,][] mapData;
        private List<GroundItem> Items;
        private List<ItemSpawn> ItemSpawns;

        public byte MapID { get; private set; }
        public byte Brightness { get; private set; }

        public Map(byte mapID)
        {
            MapID = mapID;
            loaded = false;
            Items = new List<GroundItem>();
            if (MapID != 0)
            {
                //Load map
                Thread mapReadThread = new Thread(ReadMapFile);
                mapReadThread.Start();
                //Load mapItems
                Thread mapItemLoadThread = new Thread(LoadMapItems);
                mapItemLoadThread.Start();
            }
        }

        public IMobile GetMobile(int mobileID)
        {
            return null;
        }

        public void AddProjectile(ProjectTileType ProjectTileType, Point3D location, byte dir) { }

        public void GetMapSector(IMobile mobile, short sector) { }
        public void GetMapPatch(IMobile mobile, short sector) { }
        public void UpdateData(short sector, byte[] data) { }

        public void OpenDoor(IMobile mobile, Point3D location)
        {
            if (mapData[location.X, location.Y].Length < location.Z) return;
            if (!loaded) return;
            mapData[location.X, location.Y][location.Z].DoorStatus = DoorStatus.Open;

            //Broadcast packet...
            GetDoorStatus(mobile, location);
        }

        public void GetDoorStatus(IMobile mobile, Point3D location)
        {
            if (mapData[location.X, location.Y].Length < location.Z) return;
            //Set all doors to open...
            MapData md = mapData[location.X, location.Y][location.Z];
            mobile.PlayerSocket.Send(new DoorStatusPacket(this, location, md.DoorStatus));
        }

        public bool CanMove(IMobile mobile, Point3D location) 
        {
            if (mapData[location.X, location.Y].Length < location.Z) return false;

            MapData md = mapData[location.X, location.Y][location.Z];
            //has an Item cannot move
            if (md.item1 > 0)
            {
                Console.WriteLine("Tried walking on Tile with id of {0}", md.item1);
                //TODO: Load resource file to get correct info
                //pass back true for now
                return true;
            }
            return true; 
        }

        public void ProccessMovement(IMobile mobile, Point3D location) 
        {
            if (mapData[location.X, location.Y].Length < location.Z) return;
            MapData md = mapData[location.X, location.Y][location.Z];
            if (!string.IsNullOrEmpty(md.Script))
            {
                MapScriptHandler handler = MapScriptManager.GetScriptHandler(md.Script);
                if (handler == null)
                {
                    Console.WriteLine("Unhandled Script {0}", md.Script);
                }
                else
                {
                    try
                    {
                        handler.ProcessScript(mobile);
                    }
                    catch
                    {
                        Console.WriteLine("Error processing Script {0} for {1}", md.Script, mobile);
                    }
                }
            }
            if (!string.IsNullOrEmpty(md.MapCommand))
            {
                string mapCommand = md.MapCommand;
                if (mapCommand.StartsWith("W"))
                {
                    string[] newLocation = mapCommand.Replace("W", "").Trim().Split(' ');
                    int X = byte.Parse(newLocation[0]) + ((byte.Parse(newLocation[1])) * 256);
                    int Y = byte.Parse(newLocation[2]) + ((byte.Parse(newLocation[3])) * 256);
                    byte M = byte.Parse(newLocation[4]);
                    byte Z = byte.Parse(newLocation[5]);

                    IMap targetMap = MapManager.GetMap(M);
                    Point3D targetLocation = new Point3D(X, Y, Z);
                    mobile.WarpToLocation(targetMap, targetLocation);
                    return;
                }
                if (mapCommand.StartsWith("B"))
                {
                    mobile.OpenBank();
                    return;
                }

                if (mapCommand.StartsWith("!")) return; //additional map info
                if (mapCommand.StartsWith("x")) return; //additional map info
                if (mapCommand.StartsWith("z")) return; //additional map info
                if (mapCommand.StartsWith("G")) return; //additional map info
                    Console.WriteLine(String.Format("Map Command \"{0}\" Not Implemented...", mapCommand));
            }
        }

        private void ReadMapFile()
        {
            string mapPath = Path.Combine(Core.BaseDirectory, "Data", String.Format("nm{0}.map", MapID));
            int maxLevel = 0;
            if (File.Exists(mapPath))
            {
                mapData = new MapData[512,512][];
                UInt32[,,] Addresses = new UInt32[512, 512, 9];
                if (DEBUG) Console.WriteLine("Reading map file {0}", MapID);
                using (FileStream fs = new FileStream(mapPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        UInt16 iTemp = 0;
                        UInt32 uiTemp = 0;
                        for (int mX = 0; mX < 512; mX++)
                        {
                            for (int mY = 0; mY < 512; mY++)
                            {
                                try
                                {
                                    Addresses[mX, mY, 0] = br.ReadUInt32();
                                    iTemp = br.ReadUInt16();
                                    //z level 1
                                    mapData[mX, mY] = new MapData[1];
                                    mapData[mX, mY][0].icon1 = (Int16)(iTemp & 8191);
                                    mapData[mX, mY][0].icon1index = (byte)(iTemp / 16384);

                                    iTemp = br.ReadUInt16();
                                    //z level 1 also
                                    mapData[mX, mY][0].icon2 = (Int16)(iTemp & 8191);
                                    mapData[mX, mY][0].icon2index = (byte)(iTemp / 16384);
                                }
                                catch { }
                            }
                        }
                        for (int mX = 0; mX < 512; mX++)
                        {
                            for (int mY = 0; mY < 512; mY++)
                            {
                                for (int z = 0; z <= 7; z++)
                                {
                                    if (Addresses[mX, mY, z] != 0)
                                    {
                                        if (mapData[mX, mY].Length < z + 1)
                                        {
                                            Array.Resize(ref mapData[mX, mY], z + 1);
                                        }
                                        if (z > maxLevel) maxLevel = z;
                                        try
                                        {
                                            fs.Seek(Addresses[mX, mY, z], SeekOrigin.Begin);
                                            uiTemp = br.ReadUInt32();
                                            if (uiTemp != 0)
                                                Addresses[mX, mY, z + 1] = uiTemp;

                                            iTemp = br.ReadByte();
                                            if (iTemp == 0)//do nothing 
                                            {
                                                for (int i = 0; i < 13; i++)
                                                {
                                                    iTemp = br.ReadByte();
                                                }
                                            }
                                            else if (iTemp == 38)
                                            {
                                                iTemp = br.ReadUInt16();
                                                mapData[mX, mY][z].item2 = (Int16)iTemp;

                                                iTemp = br.ReadByte();
                                                mapData[mX, mY][z].item2index = (byte)iTemp;

                                                for (int i = 0; i < 10; i++)
                                                    iTemp = br.ReadByte();
                                            }
                                            else if (iTemp == 115)//115 script
                                            {
                                                string script = "";
                                                for (int i = 0; i <= 12; i++)
                                                {
                                                    iTemp = br.ReadByte();
                                                    if (iTemp != 0)
                                                        script += (char)iTemp;
                                                }
                                                mapData[mX, mY][z].Script = script;
                                            }
                                            else
                                            {
                                                string mapcommand = ((char)iTemp).ToString();
                                                for (int i = 0; i <= 12; i++)
                                                {
                                                    iTemp = br.ReadByte();
                                                    mapcommand += string.Format(" {0}", iTemp);
                                                }
                                                mapData[mX, mY][z].MapCommand = mapcommand;
                                            }

                                            iTemp = br.ReadUInt16();
                                            mapData[mX, mY][z].item1index = (byte)(iTemp / 256);

                                            iTemp = br.ReadUInt16();
                                            mapData[mX, mY][z].item1 = (byte)(iTemp / 256);

                                            iTemp = br.ReadUInt16();
                                            //floor of next z level
                                            if (iTemp != 0)
                                            {
                                                if (mapData[mX, mY].Length < z + 2)
                                                    Array.Resize(ref mapData[mX, mY], z + 2);
                                                mapData[mX, mY][z + 1].icon1 = (Int16)(iTemp & 8191);
                                                mapData[mX, mY][z + 1].icon1index = (byte)(iTemp / 16384);
                                            }
                                        } 
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Invalid Data in map {0} {1}", MapID, ex.Message);
                                        }
                                    }
                                }
                            }
                        }
                        br.Close();
                    }
                    fs.Close();
                }
                Addresses = null;
                loaded = true;
                if (INFO) Console.WriteLine("Map Data Loaded {0}", MapID);
            }
            else
            {
                Console.WriteLine("Map file {0} Missing", MapID);
            }
        }

        private void LoadMapItems()
        {
            ItemSpawns = new List<ItemSpawn>();
            if (DEBUG) Console.WriteLine("Loading map Items {0}", MapID);

            SQLiteCommand sqlCommand = new SQLiteCommand("Select id, itemID, x, y, z, CanPickup from MapItem " +
                "where map = :map", DataManager.Connection);
            sqlCommand.Parameters.Add(new SQLiteParameter("map", MapID));

            using (SQLiteDataReader dr = sqlCommand.ExecuteReader())
            {
                while (dr.Read())
                {
                    try
                    {
                        ItemSpawn itemSpawn = new ItemSpawn();
                        int index = 0;
                        itemSpawn.LocationID = (byte)dr.GetInt32(index++);
                        itemSpawn.ItemID = (Int16)dr.GetInt32(index++);
                        itemSpawn.X = (Int16)dr.GetInt32(index++);
                        itemSpawn.Y = (Int16)dr.GetInt32(index++);
                        itemSpawn.Z = (byte)dr.GetInt32(index++);
                        itemSpawn.CanPickup = dr.GetInt32(index++) == 0 ? false : true;

                        ItemSpawns.Add(itemSpawn);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("LoadMapItems error: {0}", ex.Message);
                    }
                }
            }

            if (INFO) Console.WriteLine("Map Items Loaded {0}", MapID);
        }

        public static void CreateData()
        {
            SQLiteCommand dropCommand = new SQLiteCommand("DROP Table IF EXISTS MapItem", DataManager.Connection);
            dropCommand.ExecuteNonQuery();

            string command = "create table MapItem " +
                "(map INTEGER NOT NULL, " +
                "id INTEGER NOT NULL, " +
                "itemID INTEGER NOT NULL, " +
                "x INTEGER NOT NULL, " +
                "y INTEGER NOT NULL, " +
                "z INTEGER NOT NULL, " +
                "CanPickup INTEGER NOT NULL," +
                "PRIMARY KEY(map,id))";//0 = false, 1 = true
            SQLiteCommand sqlCommand = new SQLiteCommand(command, DataManager.Connection);
            sqlCommand.ExecuteNonQuery();

            sqlCommand = new SQLiteCommand("CREATE UNIQUE INDEX idx_mapitem_map ON mapitem(map)", DataManager.Connection);
            sqlCommand.ExecuteNonQuery();
        }
    }
}
