using System;
using System.Collections.Generic;
using System.Text;

namespace MaldonServer.Network.ServerPackets
{
    public class Stage1 : Packet
    {
        //public static int packetData = 0;
        public Stage1() : base(0x52, 6)
        {
            Write((byte)Utility.Random(1, 254));
            Write((byte)Utility.Random(1, 254));
            Write((byte)Utility.Random(1, 254));
            Write((byte)Utility.Random(1, 254));
            Write((byte)Utility.Random(1, 254));
            Write((byte)Utility.Random(1, 254));
        }
    }
}
