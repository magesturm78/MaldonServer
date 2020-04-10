using System;
using System.Collections.Generic;
using MaldonServer;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
    public class Item : IItem
    {
        public int ItemID { get; protected set; }
        public byte Prefix { get; protected set; }
        public byte Suffix { get; protected set; }
        public int Amount { get; protected set; }
        public int DuraMax { get; protected set; }
        public int DuraMin { get; protected set; }
    }
}