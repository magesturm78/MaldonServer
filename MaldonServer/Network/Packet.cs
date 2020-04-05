using System;
using System.Collections.Generic;
using System.Text;

namespace MaldonServer.Network
{
    public class Packet
    {
        public int PacketID { get { return buffer[0]; } }
        private byte[] buffer;
        int index;
        public int Size { get; private set; }
        public bool HasData { get { return index < Size; } }

        List<byte> decryptIDs = new List<byte> { 0x47, 0x48, 0x49, 0x4A, 0x4E, 0x4F,
                                         0x53, 0x54, 0x56, 0x5B, 0x5D, 0x5E, 0x60, 0xC7 };

        public Packet(byte[] data)
        {
            if (decryptIDs.Contains(data[0]))
                buffer = DecryptPacket(data);
            else
                buffer = data;

            buffer = data;
            index = 1;
            Size = data.Length;
        }

        public Packet(byte packetID, int length)
        {
            EnsureCapacity(packetID, length);
        }

        internal void EnsureCapacity(byte packetID, int length)
        {
            buffer = new byte[length];
            buffer[0] = packetID;
            index = 1;
        }

        internal byte ReadByte()
        {
            if ((index + 1) > Size)
                return 0;

            return buffer[index++];
        }

        internal int ReadCompressed()
        {
            if ((index + 1) > Size)
                return 0;

            int value = buffer[index++];
            if (value >= 128) return value - 128;

            if ((index + 1) > Size)
                return 0;

            return (value * 256) + buffer[index++];
        }

        internal ushort ReadUInt16()
        {
            if ((index + 2) > Size)
                return 0;

            return (ushort)((buffer[index++]) | buffer[index++] << 8);
        }

        internal double ReadDouble()
        {
            if ((index + 4) > Size)
                return 0;
            double value = buffer[index++] * 8000000;
            value += buffer[index++] * 32768;
            value += buffer[index++];
            value += buffer[index++] * 256;

            return value;
        }

        internal string ReadString(int stringLength)
        {
            int bound = index + stringLength;
            int end = bound;

            if (bound > Size)
                bound = Size;

            StringBuilder sb = new StringBuilder();

            int c;

            while (index < bound && (c = buffer[index++]) != 0)
                sb.Append((char)c);

            index = end;

            return sb.ToString();
        }

        internal string ReadNullString()
        {
            int end = Size;

            StringBuilder sb = new StringBuilder();

            int c;

            while (index < end && (c = buffer[index++]) != 0)
                sb.Append((char)c);

            index = end;

            return sb.ToString();
        }

        private byte[] DecryptPacket(byte[] data)
        {
            byte[] dec_Data = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                int x = data[i];
                int r1 = x;
                int position = i;
                if ((position % 2) == 1) r1 += (((x + 1) % 2) * 2);
                if (((position / 2) % 2) == 1) r1 += ((((x / 2) + 1) % 2) * 4);
                if (((position / 4) % 2) == 1) r1 += ((((x / 4) + 1) % 2) * 8);
                if (((position / 8) % 2) == 1) r1 += ((((x / 8) + 1) % 2) * 16);
                if (((position / 16) % 2) == 1) r1 += ((((x / 16) + 1) % 2) * 32);
                if (((position / 32) % 2) == 1) r1 += ((((x / 32) + 1) % 2) * 64);
                if (((position / 64) % 2) == 1) r1 += ((((x / 64) + 1) % 2) * 128);

                r1 -= position;

                dec_Data[position] = (byte)r1;
            }
            return dec_Data;
        }

        public void Write(byte data)
        {
            buffer[index] = data;
            index++;
        }

        public byte[] Compile()
        {
            return buffer;
        }
    }
}
