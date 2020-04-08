using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public interface IAccountManager
    {
        void CreateAccount(PlayerSocket ps, string un, string pw, string em);
        void LoginAccount(PlayerSocket ps, string un, string pw);
    }
}
