using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer.Network
{
    public delegate void OnPacketReceive(PlayerSocket socket, Packet packet);

    public class PacketHandler
    {
        public int PacketID { get; private set; }
        public bool InGame { get; private set; }
        public OnPacketReceive OnReceive { get; private set; }

        public PacketHandler(int packetID, bool ingame, OnPacketReceive onReceive)
        {
            PacketID = packetID;
            InGame = ingame;
            OnReceive = onReceive;
        }
    }
}
