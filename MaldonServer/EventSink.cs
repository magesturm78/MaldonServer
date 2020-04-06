using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;
using System;
using System.Net.Sockets;

namespace MaldonServer
{
    public delegate void ServerStartedEventHandler();
    public delegate void ShutdownEventHandler();

    public delegate void SocketConnectEventHandler(SocketConnectEventArgs e);

    public delegate void AccountCreateEventHandler(AccountCreateEventArgs e);
    public delegate void AccountLoginEventHandler(AccountLoginEventArgs e);

    public class SocketConnectEventArgs : EventArgs
    {
        public Socket Socket { get; private set; }
        public bool AllowConnection { get; set; }

        public SocketConnectEventArgs(Socket s)
        {
            Socket = s;
            AllowConnection = true;
        }
    }

    public class AccountCreateEventArgs : EventArgs
    {
        public PlayerSocket PlayerSocket { get; }
        public string Username { get; }
        public string Password { get; }
        public string Email { get; }
        public bool Accepted { get; set; }
        public ALRReason Reply { get; set; }

        public AccountCreateEventArgs(PlayerSocket ps, string un, string pw, string em)
        {
            PlayerSocket = ps;
            Username = un;
            Password = pw;
            Email = em;
        }
    }

    public class AccountLoginEventArgs : EventArgs
    {
        public PlayerSocket PlayerSocket { get; }
        public string Username { get; }
        public string Password { get; }
        public bool Accepted { get; set; }
        public ALRReason Reply { get; set; }

        public AccountLoginEventArgs(PlayerSocket ps, string un, string pw)
        {
            PlayerSocket = ps;
            Username = un;
            Password = pw;
        }
    }

    public class EventSink
    {
        public static event ServerStartedEventHandler ServerStarted;
        public static event ShutdownEventHandler ServerShutdown;

        public static event SocketConnectEventHandler SocketConnect;

        public static event AccountCreateEventHandler AccountCreate;
        public static event AccountLoginEventHandler AccountLogin;

        public static void InvokeServerStarted()
        {
            ServerStarted?.Invoke();
        }

        public static void InvokeServerShutdown()
        {
            ServerShutdown?.Invoke();
        }

        public static void InvokeSocketConnect(SocketConnectEventArgs e)
        {
            SocketConnect?.Invoke(e);
        }

        public static void InvokeAccountCreate(AccountCreateEventArgs e)
        {
            AccountCreate?.Invoke(e);
        }

        public static void InvokeAccountLogin(AccountLoginEventArgs e)
        {
            AccountLogin?.Invoke(e);
        }
    }
}
