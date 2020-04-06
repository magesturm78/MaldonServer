using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public class MapManager
    {
        private static Hashtable maps;

        public static Map Internal { get { return (Map)maps[0]; } }

        public static Map GetMap(int mapID)
        {
            return (Map)maps[mapID];
        }

        static MapManager()
        {
            maps.Add(0, null);//Internal Map
        }
    }
}
