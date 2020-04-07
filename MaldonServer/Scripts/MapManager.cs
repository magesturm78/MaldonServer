using System;
using System.Collections;
using System.Collections.Generic;

namespace MaldonServer.Scripts
{
    public class MapManager
    {
        private static readonly Hashtable maps;

        public static Map Internal { get { return (Map)maps[0]; } }

        public static Map GetMap(int mapID)
        {
            return (Map)maps[mapID];
        }

        public static void AddMap(Map map)
        {
            maps[map.MapID] = map;
        }

        static MapManager()
        {
            maps = new Hashtable
            {
                { 0, null }//Internal Map
            };
        }
    }
}
