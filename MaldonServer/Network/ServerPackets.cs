using System;
using System.Collections.Generic;
using System.Text;

namespace MaldonServer.Network.ServerPackets
{
    public class Stage1 : Packet
    {
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

    public class Stage1Reply : Packet
    {
        public Stage1Reply() : base(0x35, 6)
        {
            Write((byte)(0x30 + Utility.Random(0, 9)));
            Write((byte)(0x30 + Utility.Random(0, 9)));
            Write((byte)(0x30 + Utility.Random(0, 9)));
            Write((byte)(0x30 + Utility.Random(0, 9)));
            Write((byte)(0x30 + Utility.Random(0, 9)));
            Write((byte)(0x30 + Utility.Random(0, 9)));
        }
    }
}
