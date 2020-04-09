using MaldonServer.Network.ServerPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public interface IMap
    {
        byte MapID { get; }
        byte Brightness { get; }

        IMobile GetMobile(int mobileID);
        void AddProjectile(ProjectTileType ProjectTileType, Point3D location, byte dir);
        void GetDoorStatus(IMobile mobile, Point3D location);
        void GetMapSector(IMobile mobile, short sector);
        void UpdateData(short sector, byte[] data);
        void RecomputeCheckSum(short sector);
        bool CanMove(IMobile mobile, Point3D location);
        void ProccessMovement(IMobile mobile, Point3D location);

    }
}
