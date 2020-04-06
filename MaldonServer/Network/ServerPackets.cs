using System;
using System.Collections.Generic;
using System.Text;

namespace MaldonServer.Network.ServerPackets
{
    #region Client Initialization Packets
    public sealed class Stage1 : Packet
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

    public sealed class Stage1Reply : Packet
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

    public sealed class Stage2Reply : Packet
    {
        public Stage2Reply() : base(0x3F, 0)
        {
        }
    }

    public sealed class Stage2Reply2 : Packet
    {
        public Stage2Reply2() : base(0xBF, 0)
        {
        }
    }
    #endregion

    #region account packets
    public enum ALRReason : byte
    {
        AccountExists   = 0x00,
        Blocked         = 0x01,//notused
        AccountCreated  = 0x02,
        InvalidAccount  = 0x03,
        BadPass         = 0x04,
        CorrectPassword = 0x05,
        CharExist       = 0x08,
        CharInvPw       = 0x21,
        CharInUse       = 0x6A,
    }

    public sealed class AccountCreateLoginReply : Packet
    {
        public AccountCreateLoginReply(ALRReason reason) : base(0x65, 1)
        {
            Write((byte)reason);
        }
    }
    #endregion
}
