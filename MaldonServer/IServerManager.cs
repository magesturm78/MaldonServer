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
    }
}
