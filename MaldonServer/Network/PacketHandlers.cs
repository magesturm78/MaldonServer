using MaldonServer.Network.ServerPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer.Network
{
    public class PacketHandlers
    {
        public static PacketHandler[] Handlers { get; private set; }

        static PacketHandlers()
        {
            Handlers = new PacketHandler[byte.MaxValue];
            Register(0x32, false, new OnPacketReceive(Stage1Responce));

        }

        private static void Register(int packetID, bool ingame, OnPacketReceive onReceive)
        {
            Handlers[packetID] = new PacketHandler(packetID, ingame, onReceive);
        }

        public static void Stage1Responce(PlayerSocket socket, Packet packet)
        {
            socket.Send(new Stage1Reply());
        }

        internal static PacketHandler GetHandler(byte packetID)
        {
            return Handlers[packetID];
        }
    }
}
