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

            //Client initiailization packets
            Register(0x32, false, new OnPacketReceive(Stage1Responce));
            Register(0x6D, false, new OnPacketReceive(Stage2));
            Register(0x29, false, new OnPacketReceive(Stage2Responce));//Unknown

            //Account packets
            Register(0x63, false, new OnPacketReceive(AccountPackets));
            Register(0x02, false, new OnPacketReceive(LoginCharacter));
            Register(0x03, false, new OnPacketReceive(CreateCharacter));

            //Misc Packets
            Register(0x16, false, new OnPacketReceive(PingReq));
        }

        #region Client initialization packets
        public static void Stage1Responce(PlayerSocket socket, Packet packet)
        {
            socket.Send(new Stage1Reply());
        }

        public static void Stage2(PlayerSocket socket, Packet packet)
        {
            socket.Send(new Stage2());
        }

        public static void Stage2Responce(PlayerSocket socket, Packet packet)
        {
            socket.Send(new Stage2Reply());
        }
        #endregion

        public static void PingReq(PlayerSocket socket, Packet packet)
        {
            //do nothing just keep client alive...
        }

        private static void AccountPackets(PlayerSocket socket, Packet packet)
        {
            byte accountAction = packet.ReadByte();
            string username;
            string password;
            string email;
            switch (accountAction)
            {
                case 0x00://Create account
                    username = packet.ReadString(15).Trim();
                    password = packet.ReadString(15).Trim();
                    email = packet.ReadString(30).Trim();

                    AccountCreateEventArgs ce = new AccountCreateEventArgs(socket, username, password, email);

                    Console.WriteLine("Login: {0}: AccountCreate", socket);
                    EventSink.InvokeAccountCreate(ce);

                    socket.Send(new AccountCreateLoginReplyPacket(ce.Reply));
                    break;
                case 0x01://Login account
                    username = packet.ReadString(15).Trim();
                    password = packet.ReadString(15).Trim();

                    AccountLoginEventArgs le = new AccountLoginEventArgs(socket, username, password);

                    Console.WriteLine( "Login: {0}: AccountLogin", socket );
                    EventSink.InvokeAccountLogin(le);

                    socket.Send(new AccountCreateLoginReplyPacket(le.Reply));
                    break;
                case 0x03://Get character List
                    socket.Send(new CharacterListPacket(socket.Account));
                    break;
                default:
                    Console.WriteLine("Login: {0}: Unknow Account Action {1}", socket, accountAction);
                    break;
            }
        }

        public static void CreateCharacter(PlayerSocket socket, Packet packet)
        {
            string name = packet.ReadString(12).Trim();
            string password = packet.ReadString(12).Trim();

            packet.ReadByte(); packet.ReadByte(); packet.ReadByte(); packet.ReadByte();// 0x00 0x14 0x14 0x14 
            packet.ReadByte(); packet.ReadByte(); packet.ReadByte(); packet.ReadByte();// 0x14 0x14 0x14 0x00
            byte gender = packet.ReadByte();

            string name2 = packet.ReadUnicodeString(12);
            packet.ReadByte(); packet.ReadByte(); packet.ReadByte();// 0x04 0x00 0x00
            byte hairID = packet.ReadByte();

            Console.WriteLine("CreateChar: {0}: start", socket);

            CharacterCreateEventArgs args = new CharacterCreateEventArgs(socket, name, password, gender, hairID);

            EventSink.InvokeCharacterCreate(args);
        }

        public static void LoginCharacter(PlayerSocket socket, Packet packet)
        {
            string name = packet.ReadString(12).Trim();
            string password = packet.ReadString(12).Trim();

            EventSink.InvokeCharacterLogin(new CharacterLoginEventArgs(socket, name, password));
        }

        #region Class functions
        private static void Register(int packetID, bool ingame, OnPacketReceive onReceive)
        {
            Handlers[packetID] = new PacketHandler(packetID, ingame, onReceive);
        }

        internal static PacketHandler GetHandler(byte packetID)
        {
            return Handlers[packetID];
        }
        #endregion
    }
}
