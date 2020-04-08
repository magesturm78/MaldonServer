using System;
using System.Collections.Generic;
using MaldonServer;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
    public class PlayerMobile : Mobile
    {
        public PlayerMobile(): base()
        {
            Map = MapManager.Internal;
            SpawnLocation = new SpawnLocation(MapManager.GetMap(7), new Point3D(343, 91, 0));
        }

        public void Spawn()
        {
            Map = SpawnLocation.Map;
            SetLocation(SpawnLocation.Location, true);
        }

        public void MoveToWorld(Point3D newLocation, IMap map)
        {
            Map = map;
            X = newLocation.X;
            Y = newLocation.Y;
            Z = newLocation.Z;

        }
    }
}