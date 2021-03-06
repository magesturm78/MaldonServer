﻿using MaldonServer.Network;
using System;
using System.Net.Sockets;

namespace MaldonServer
{
    public interface IServerManager
    {
        void Start();
        void Shutdown();
        bool AllowConnection(Socket s);
        void UploadScript(PlayerSocket playerSocket, NPCSpawnInfo spawnInfo);
        void DownloadScript(PlayerSocket playerSocket, string filename);
        void GetSpawnInfo(PlayerSocket playerSocket, byte mapID, int mobileID);
        void AddSpawn(PlayerSocket socket, MobileSpawn ms);
        void InvalidDataFromClient(string address, Exception ex);
        void PacketHandlerError(string address, Exception ex);
    }
}
