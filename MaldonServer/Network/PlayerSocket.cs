using MaldonServer.Network.ServerPackets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MaldonServer.Network
{
    public class PlayerSocket
    {
        private Socket socket;
        private bool disposing;
        private byte[] recvBuffer;
        private byte[] recvBuffer2;

        private DateTime nextCheckActivity;
        private string Address;
        private Queue<Packet> packets;

        private AsyncCallback onReceive, onSend;

        /// <summary>
        /// 0 - 256
        /// </summary>
        public byte SocketID { get; private set; }
        public bool Running { get; private set; }
        public IAccount Account;
        public IMobile Mobile;

        public PlayerSocket(Socket socket, byte socketID)
        {
            this.socket = socket;
            SocketID = socketID;
            Running = false;
            recvBuffer = new byte[2048];
            packets = new Queue<Packet>();

            nextCheckActivity = DateTime.Now + TimeSpan.FromSeconds(15);

            try { Address = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(); }
            catch { Address = "";}

            Start();
        }

        private void Start()
        {
            onReceive = new AsyncCallback(OnReceive);
            onSend = new AsyncCallback(OnSend);

            Running = true;

            if (socket == null)
                return;

            try
            {
                socket.BeginReceive(recvBuffer, 0, 2048, SocketFlags.None, onReceive, null);
                Send(new Stage1());
                //SentFirstPacket = true;
            }
            catch( Exception ex )
            {
                Console.WriteLine(ex);
                Dispose();
            }
        }

        internal void Continue()
        {
            //HandlePackets
            lock (packets)
            {
                while (packets.Count > 0)
                {
                    Packet p = packets.Dequeue();
                    PacketHandler handler = PacketHandlers.GetHandler(p.PacketID);
                    if (handler == null)
                    {
                        Console.WriteLine("Unhandled Packet 0x{0:X2} {1}", p.PacketID, DateTime.Now.ToString("HH:mm:ss"));
                    }
                    else
                    {
                        //Console.WriteLine("Recieved Packet 0x{0:X2} {1}", p.PacketID, DateTime.Now.ToString("HH:mm:ss"));
                        handler.OnReceive(this, p);
                    }
                }
            }
        }

        private void OnReceive(IAsyncResult asyncResult)
        {
            lock (this)
            {
                if (socket == null)
                    return;

                try
                {
                    int byteCount = socket.EndReceive(asyncResult);

                    if (byteCount > 0)
                    {
                        nextCheckActivity = DateTime.Now + TimeSpan.FromSeconds(15);

                        //Console.WriteLine("Client: {0}: recieved data length {1}", this, byteCount);

                        HandleData(byteCount);

                        Listener.Instance.QueueSocket(SocketID);
                    }
                    else
                    {
                        Console.WriteLine("Client Disconnected from server.");
                        Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Dispose();
                }
                finally
                {
                    if (socket != null)
                        socket.BeginReceive(recvBuffer, 0, 2048, SocketFlags.None, onReceive, null);
                }
            }
        }

        private void OnSend(IAsyncResult asyncResult)
        {
            if (socket == null)
                return;

            try
            {
                int bytes = socket.EndSend(asyncResult);

                if (bytes <= 0)
                {
                    Dispose();
                    return;
                }

                //Console.WriteLine( "OnSend: {0}: Complete send of {1} bytes", this, bytes );
                //nextCheckActivity = DateTime.Now + TimeSpan.FromSeconds(15);
            }
            catch // ( Exception ex )
            {
                //Console.WriteLine(ex);
                Dispose();
            }
        }

        public void Dispose()
        {
            if (socket == null || disposing)
                return;

            disposing = true;

            try { socket.Shutdown(SocketShutdown.Both); }
            catch { }

            try { socket.Close(); }
            catch { }

            socket = null;
            recvBuffer = null;
            onReceive = null;
            onSend = null;
            Running = false;

            Listener.Instance.RemoveSocket(SocketID);
        }

        private void HandleData(int size)
        {
            byte[] buffer;// = new byte[MAX_PACKET_SIZE*2];
            int position = 0;
            int length;
            int id_loc;

            if (recvBuffer2 != null)
            {
                buffer = new byte[size + recvBuffer2.Length];
                Array.Copy(recvBuffer2, 0, buffer, 0, recvBuffer2.Length);
                Array.Copy(recvBuffer, 0, buffer, recvBuffer2.Length, size);
                size += recvBuffer2.Length;
                recvBuffer2 = null;
            }
            else
            {
                buffer = new byte[size];
                Array.Copy(recvBuffer, 0, buffer, 0, size);
            }
            while (position < size)
            {
                if (size > 1)
                {
                    length = buffer[position];
                    id_loc = position + 1;
                    if (length < 128)
                    {
                        length = (length * 121) + buffer[position + 1] + 1;
                        id_loc = 2;
                    }
                    else
                    {
                        length -= 127;
                    }

                    if (position + id_loc + length > size)
                    {
                        try
                        {
                            //create modular static buffer to store then add it local buffer before adding new data next run
                            recvBuffer2 = new byte[size - position];
                            Array.Copy(buffer, position, recvBuffer2, 0, size - position);
                        }
                        catch
                        {
                            //Debug.Log(String.Format("Error 2 {0}", ex.Message));
                        }
                    }
                    else
                    {
                        byte[] nreBuffer = new byte[length];
                        {
                            Array.Copy(buffer, id_loc, nreBuffer, 0, length);
                            packets.Enqueue(new Packet(nreBuffer));
                        }
                    }
                    position = id_loc + length;
                }
            }
        }

        public void Send(Packet p)
        {
            if (socket == null || p == null)
                return;

            byte[] buffer = p.Compile();
            //Console.WriteLine("Sending Packet 0x{0:X2} {1}", p.PacketID, DateTime.Now.ToString("HH:mm:ss"));

            if (buffer != null)
            {
                if (buffer.Length <= 0)
                    return;

                int length = buffer.Length;

                try { 
                    socket.BeginSend(buffer, 0, length, SocketFlags.None, onSend, null);
                }
                catch // ( Exception ex )
                {
                    //Console.WriteLine(ex);
                    Dispose();
                }
            }
        }

        public override string ToString()
        {
            return Address;
        }
    }
}
