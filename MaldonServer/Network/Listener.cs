using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MaldonServer.Network
{
    public class Listener
    {
		public static Listener Instance { get; private set; }

		public const byte MaxConnections = byte.MaxValue;

		public int Port { get; private set; }
		public static int ConnectedCount { get; private set; }

		private Socket listener;
		private bool disposed;
		private readonly AsyncCallback onAccept;
		public PlayerSocket[] PlayerSockets { get; private set; }
		private readonly Queue<int> socketsQueue;

		public Listener(int port)
		{
			Instance = this;
			PlayerSockets = new PlayerSocket[MaxConnections];

			Port = port;
			disposed = false;
			onAccept = new AsyncCallback(OnAccept);
			socketsQueue = new Queue<int>();

			listener = Bind(IPAddress.Any, port);

			try
			{
				IPHostEntry iphe = Dns.GetHostEntry(Dns.GetHostName());

				ArrayList list = new ArrayList
				{
					IPAddress.Loopback
				};

				Console.WriteLine("Address: {0}:{1}", IPAddress.Loopback, port);

				IPAddress[] ips = iphe.AddressList;

				for (int i = 0; i < ips.Length; ++i)
				{
					if (!list.Contains(ips[i]))
					{
						if (ips[i].AddressFamily == AddressFamily.InterNetwork)//IPv4 only
						{
							list.Add(ips[i]);

							Console.WriteLine("Address: {0}:{1}", ips[i], port);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format("Recieved error in Listener.Listener {0}", ex.Message));
			}
		}

		private Socket Bind(IPAddress ip, int port)
		{
			IPEndPoint ipep = new IPEndPoint(ip, port);

			Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				s.Bind(ipep);
				s.Listen(300);

				s.BeginAccept(onAccept, s);
				return s;
			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format("Recieved error in Listener.Bind {0}", ex.Message));

				try { s.Shutdown(SocketShutdown.Both); }
				catch { }

				try { s.Close(); }
				catch { }

				return null;
			}
		}

		private void OnAccept(IAsyncResult asyncResult)
		{
			try
			{
				Socket s = listener.EndAccept(asyncResult);
                if (World.ServerManager.AllowConnection(s))
                {
                    for (byte i = 1; i < MaxConnections; i++)
                    {
                        if (PlayerSockets[i] == null)
                        {
                            PlayerSockets[i] = new PlayerSocket(s, i);
                            ConnectedCount++;
                            break;
                        }
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format("Recieved error in Listener.OnAccept {0}", ex.Message));
			}
			finally
			{
				listener.BeginAccept(onAccept, listener);
			}
		}

        internal void QueueSocket(int socketID)
		{
			lock (socketsQueue)
			{
				socketsQueue.Enqueue(socketID);
			}
		}

		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;

				if (listener != null)
				{
					try { listener.Shutdown(SocketShutdown.Both); }
					catch { }

					try { listener.Close(); }
					catch { }

					listener = null;
				}
			}
		}

		public void Slice()
		{
			lock (socketsQueue)
			{
				while (socketsQueue.Count > 0)
				{
					int i = socketsQueue.Dequeue();

					if (PlayerSockets[i].Running)
						PlayerSockets[i].Continue();
				}
			}
		}

		internal void RemoveSocket(byte socketID)
        {
            PlayerSockets[socketID] = null;
			ConnectedCount--;
		}

    }
}
