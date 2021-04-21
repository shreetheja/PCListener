using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class Client
{
    public static int dataBufferSize = 4096;
    public string username;
    public string UsedPassword;
    public int id;
    public TCP tcp;
    public UDP udp;

    public Client(int _ClientId)
    {
        id = _ClientId;
        tcp = new TCP(id);
        udp = new UDP(id);
    }

    public class TCP
    {
        public TcpClient socket;
        private readonly int id;
        private NetworkStream stream;
        private byte[] recieveBuffer;
        private Packet recievedData;
        public TCP(int _id)
        {
            id = _id;
        }
        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();

            recievedData = new Packet();
            recieveBuffer = new byte[dataBufferSize];

            stream.BeginRead(recieveBuffer, 0, dataBufferSize, RecieveCallback, null);
            ServerSend.Welcome(id, "Enter The username and Password");
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error Sending data to Player {id} via TCP : {_ex}");
            }
        }

        private void RecieveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Server.clients[id].disconnect();
                }
                byte[] _data = new byte[_byteLength];
                Array.Copy(recieveBuffer, _data, _byteLength);


                recievedData.Reset(HandleData(_data));

                stream.BeginRead(recieveBuffer, 0, dataBufferSize, RecieveCallback, null);
            }
            catch (Exception _EX)
            {
                Console.WriteLine($"Error Recieving TCP Data Or end Of connection Exception tr is : {_EX}");
                Server.clients[id].disconnect();
            }
        }
        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;
            recievedData.SetBytes(_data);
            if (recievedData.UnreadLength() >= 4)
            {
                _packetLength = recievedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }
            while (_packetLength > 0 && _packetLength <= recievedData.UnreadLength())
            {
                byte[] _packetBytes = recievedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.PacketHandlers[_packetId](id, _packet);
                    }

                });
                _packetLength = 0;
                if (recievedData.UnreadLength() >= 4)
                {
                    _packetLength = recievedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

            }
            if (_packetLength <= 1)
                return true;

            return false;
        }
        public void disconnect()
        {
            socket.Close();
            stream = null;
            recievedData = null;
            recieveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        // public UdpClient socket;
        public IPEndPoint endpoint;
        private int id;

        public UDP(int _id)
        {
            id = _id;
        }

        public void Connect(IPEndPoint _endpoint)
        {
            endpoint = _endpoint;
        }
        public void SendData(Packet _packet)
        {
            Server.SendUDPData(endpoint, _packet);
        }
        public void HandleData(Packet _packetData)
        {
            int _packetLength = _packetData.ReadInt();
            byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_packetBytes))
                {
                    int _packetId = _packet.ReadInt();
                    Server.PacketHandlers[_packetId](id, _packet);
                }

            });
        }
        public void disconnect()
        {
            endpoint = null;
        }
    }




    public void disconnect()
    {
        Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} Has Disconnected...");
        ThreadManager.ExecuteOnMainThread(() =>
        {
            //any inistilised things destroy
        }
        );
        tcp.disconnect();
        udp.disconnect();
    }
}

