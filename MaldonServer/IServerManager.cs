using MaldonServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
    }
}
