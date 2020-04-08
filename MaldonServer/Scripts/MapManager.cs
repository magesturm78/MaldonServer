using System;
using System.Collections;
using System.Collections.Generic;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
    public class Map: IMap
    {
        public byte MapID { get; private set; }
        public byte Brightness { get; private set; }

        public Map(byte mapID)
        {
            MapID = mapID;
        }

        public IMobile GetMobile(int mobileID)
        {
            return null;
        }

        public void AddProjectile(ProjectTileType ProjectTileType, Point3D location, byte dir) { }
        public void GetDoorStatus(IMobile mobile, Point3D location) { }
        public void GetMapSector(IMobile mobile, short loc1) { }
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

        static MapManager()
        {
            AddMap(new Map(0));
        }
    }
}
