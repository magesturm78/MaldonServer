using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
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

        public static void Configure()
        {
            Console.WriteLine("Configureing Map Manager ");
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
