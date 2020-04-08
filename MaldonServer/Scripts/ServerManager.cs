using System;
using System.Net.Sockets;
using MaldonServer;
using MaldonServer.Network;

namespace MaldonServer.Scripts
{
    public class ServerManager : IServerManager
    {
        public static void Initialize()
        {
            World.SetServerManager(new ServerManager());
        }

        public void Start() { }
        public void Shutdown() { }
        public bool AllowConnection(Socket s) 
        {
            return true;
        }
        public void UploadScript(PlayerSocket playerSocket, NPCSpawnInfo spawnInfo) { }
        public void DownloadScript(PlayerSocket playerSocket, string filename) { }
        public void GetSpawnInfo(PlayerSocket playerSocket, byte mapID, int mobileID) { }
        public void AddSpawn(PlayerSocket socket, MobileSpawn ms) { }
    }
}
