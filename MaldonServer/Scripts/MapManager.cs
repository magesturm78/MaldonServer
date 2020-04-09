using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
    public class Map: IMap
    {
        struct MapData
        {
            public int icon1;
            public int icon1index;
            public int icon2;
            public int icon2index;
            public int item1;
            public int item1index;
            public int item2;
            public int item2index;
            public string MapCommand;
            public string Script;
            public float height;

            public UInt32 Address;
        }
        private MapData[,,] mapData = new MapData[513, 513, 10];

        public byte MapID { get; private set; }
        public byte Brightness { get; private set; }

        public Map(byte mapID)
        {
            MapID = mapID;
            ReadMapFile();
        }

        public IMobile GetMobile(int mobileID)
        {
            return null;
        }

        public void AddProjectile(ProjectTileType ProjectTileType, Point3D location, byte dir) { }
        public void GetDoorStatus(IMobile mobile, Point3D location) { }
        public void GetMapSector(IMobile mobile, short sector) { }
        public void GetMapPatch(IMobile mobile, short sector) { }
        public void UpdateData(short sector, byte[] data) { }
        public void RecomputeCheckSum(short sector) { }

        public bool CanMove(IMobile mobile, Point3D location) 
        {
            MapData md = mapData[location.X, location.Y, location.Z];
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
            MapData md = mapData[location.X, location.Y, location.Z];
            if (!string.IsNullOrEmpty(md.Script))
            {
                Console.WriteLine("Process Script {0}", md.Script);
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
                    if (mobile.Player)
                        mobile.PlayerSocket.Send(new PlayerBankPacket(mobile));
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
            if (MapID != 0)
            {
                string mapPath = Path.Combine(Core.BaseDirectory, String.Format("Data/nm{0}.map", MapID));

                if (File.Exists(mapPath))
                {
                    Console.WriteLine("Reading map file {0}", MapID);
                    using (FileStream fs = new FileStream(mapPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            for (int mX = 0; mX < 512; mX++)
                            {
                                for (int mZ = 0; mZ < 512; mZ++)
                                {
                                    try
                                    {
                                        UInt16 iTemp = 0;
                                        mapData[mX, mZ, 0].Address = br.ReadUInt32();
                                        iTemp = br.ReadUInt16();
                                        //z level 1
                                        mapData[mX, mZ, 0].icon1 = (iTemp & 8191);
                                        mapData[mX, mZ, 0].icon1index = iTemp / 16384;
                                        iTemp = br.ReadUInt16();
                                        //z level 1 also
                                        mapData[mX, mZ, 0].icon2 = (iTemp & 8191);
                                        mapData[mX, mZ, 0].icon2index = iTemp / 16384;
                                    }
                                    catch { }
                                }
                            }
                            for (int mX = 0; mX < 512; mX++)
                            {
                                for (int mZ = 0; mZ < 512; mZ++)
                                {
                                    for (int x = 0; x < 9; x++)
                                    {
                                        if (mapData[mX, mZ, x].Address != 0)
                                        {
                                            mapData[mX, mZ, x + 1].icon2 = 0;
                                            try
                                            {
                                                fs.Seek(mapData[mX, mZ, x].Address, SeekOrigin.Begin);
                                                mapData[mX, mZ, x + 1].Address = br.ReadUInt32();

                                                UInt16 iTemp = 0;
                                                iTemp = br.ReadByte();
                                                if (iTemp == 0)//do nothing 
                                                {
                                                    for (int z = 0; z < 13; z++)
                                                    {
                                                        iTemp = br.ReadByte();
                                                    }
                                                }
                                                else if (iTemp == 38)
                                                {
                                                    iTemp = br.ReadUInt16();
                                                    mapData[mX, mZ, x].item2 = (short)iTemp;
                                                    iTemp = br.ReadByte();
                                                    mapData[mX, mZ, x].item2index = (short)iTemp;

                                                    for (int z = 0; z < 10; z++)
                                                    {
                                                        iTemp = br.ReadByte();
                                                    }
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
                                                    mapData[mX, mZ, x].Script = script;
                                                }
                                                else
                                                {
                                                    string mapcommand = ((char)iTemp).ToString();
                                                    for (int z = 0; z <= 12; z++)
                                                    {
                                                        iTemp = br.ReadByte();
                                                        mapcommand += string.Format(" {0}", iTemp);
                                                    }
                                                    mapData[mX, mZ, x].MapCommand = mapcommand;
                                                }

                                                iTemp = br.ReadUInt16();
                                                mapData[mX, mZ, x].item1index = iTemp / 256;
                                                iTemp = br.ReadUInt16();
                                                mapData[mX, mZ, x].item1 = iTemp & 8191;

                                                iTemp = br.ReadUInt16();
                                                //floor of next z level
                                                mapData[mX, mZ, x + 1].icon1 = iTemp & 8191;
                                                mapData[mX, mZ, x + 1].icon1index = iTemp / 16384;
                                            } 
                                            catch
                                            {
                                                Console.WriteLine("Invalid Data in map {0}", MapID);
                                            }
                                        }
                                    }
                                }
                            }
                            br.Close();
                        }
                        fs.Close();
                    }
                } else
                {
                    Console.WriteLine("Map file {0} Missing", MapID);
                }
            }
        }
    }

    public class MapManager
    {
        public static IMap Internal { get { return World.GetMapByID(0); } }

        public static IMap GetMap(int mapID)
        {
            return World.GetMapByID(mapID);
        }

        public static void AddMap(Map map)
        {
            World.AddMap(map);
        }

        public static void Initialize()
        {
            Console.WriteLine("Initialize Map Manager ");
            AddMap(new Map(0));
            AddMap(new Map(1));
            AddMap(new Map(2));
            AddMap(new Map(3));
            AddMap(new Map(4));
            AddMap(new Map(5));
            AddMap(new Map(6));
            AddMap(new Map(7));
            AddMap(new Map(8));
            AddMap(new Map(9));
        }
    }
}
