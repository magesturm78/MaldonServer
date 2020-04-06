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

    public delegate void CharacterLoginEventHandler(CharacterLoginEventArgs e);
    public delegate void CharacterCreateEventHandler(CharacterCreateEventArgs e);

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

    public class CharacterLoginEventArgs : EventArgs
    {
        public PlayerSocket PlayerSocket { get; }
        public string Name { get; }
        public string Password { get; }

        public CharacterLoginEventArgs(PlayerSocket ps, string un, string pw)
        {
            PlayerSocket = ps;
            Name = un;
            Password = pw;
        }
    }

    public class CharacterCreateEventArgs : EventArgs
    {
        public PlayerSocket PlayerSocket { get; }
        public IMobile Mobile { get; set; }
        public string Name { get; }
        public string Password { get; }
        public byte Gender { get; }
        public byte Hair { get; }
        public bool Accepted { get; set; }
        public ALRReason RejectReason { get; set; }

        public CharacterCreateEventArgs(PlayerSocket playerSocket, string name, string password, byte gender, byte hair)
        {
            PlayerSocket = playerSocket;
            Name = name;
            Password = password;
            Gender = gender;
            Hair = hair;
        }
    }

    public class EventSink
    {
        public static event ServerStartedEventHandler ServerStarted;
        public static event ShutdownEventHandler ServerShutdown;

        public static event SocketConnectEventHandler SocketConnect;

        public static event AccountCreateEventHandler AccountCreate;
        public static event AccountLoginEventHandler AccountLogin;
        public static event CharacterCreateEventHandler CharacterCreate;
        public static event CharacterLoginEventHandler CharacterLogin;

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

        public static void InvokeCharacterCreate(CharacterCreateEventArgs e)
        {
            CharacterCreate?.Invoke(e);
        }

        public static void InvokeCharacterLogin(CharacterLoginEventArgs e)
        {
            CharacterLogin?.Invoke(e);
        }
    }
}
