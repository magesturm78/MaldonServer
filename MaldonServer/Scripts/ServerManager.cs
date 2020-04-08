using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using MaldonServer;
using MaldonServer.Network;

namespace MaldonServer.Scripts
{
    public class ServerManager : IServerManager
    {
        const int MaxInvalidData = 5;
        const int MaxPacketErrors = 10;
        Dictionary<string, int> invalidDataCount = new Dictionary<string, int>();
        Dictionary<string, int> invalidPacketCount = new Dictionary<string, int>();

        public static void Initialize()
        {
            World.SetServerManager(new ServerManager());
        }

        public void Start() { }

        public void Shutdown() { }

        public bool AllowConnection(Socket s) 
        {
            string address = ((IPEndPoint)s.RemoteEndPoint).Address.ToString();

            if (invalidDataCount.ContainsKey(address) && invalidDataCount[address] > MaxInvalidData)
                return false;

            if (invalidPacketCount.ContainsKey(address) && invalidPacketCount[address] > MaxPacketErrors)
                return false;

            return true;
        }

        public void UploadScript(PlayerSocket playerSocket, NPCSpawnInfo spawnInfo) { }

        public void DownloadScript(PlayerSocket playerSocket, string filename) { }

        public void GetSpawnInfo(PlayerSocket playerSocket, byte mapID, int mobileID) { }

        public void AddSpawn(PlayerSocket socket, MobileSpawn ms) { }

        public void InvalidDataFromClient(string address, Exception ex)
        {
            //can log excetion or process based on Type of Execption
            if (!invalidDataCount.ContainsKey(address))
                invalidDataCount.Add(address, 0);

            invalidDataCount[address]++;
            Console.WriteLine("Invalid Data from {0} {1}", address, ex.Message);
        }

        public void PacketHandlerError(string address, Exception ex)
        {
            if (!invalidPacketCount.ContainsKey(address))
                invalidPacketCount.Add(address, 0);

            invalidPacketCount[address]++;
            Console.WriteLine("Error Processing Packet from {0} {1}", address, ex.Message);
        }

    }
}
