using System;
using System.Net.Sockets;
using MaldonServer;

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
    }
}
