using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class Server
{
    ///MaxPlayer and Port are the server information which is set in program.cs through start() can be retrived
    ///but not be set outside this class
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }

    /// <summary>
    ///Every clients instances are stored in this dictionary.
    /// </summary>
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();


    public delegate void PacketHandler(int _fromClient, Packet _packet);
    /// <summary>
    ///This will store all the types of packets refering from Packets.cs class and assign a method to handle all such Packets
    /// </summary>
    public static Dictionary<int, PacketHandler> PacketHandlers;

    //Listeners for the UDP and TCP
    private static TcpListener tcpListener;
    private static UdpClient udpListener;

    /// <summary>
    /// Accepts maxplayers and port number and intialises them and serverdata's when called Like tcpListener udpListener and etc
    /// </summary>
    /// <param name="_maxPlayers">Number of player can connect to server</param>
    /// <param name="_port">Port number </param>
    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        Port = _port;

        Console.WriteLine("Server Starting......");

        InitialiseServerData();
        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UdpRecieveCallback, null);

        Console.WriteLine($"Server Started on {Port}.");

    }
    /// <summary>
    /// Helps in assigning the Client to thier respective instances in server and stores that instance in the dictionary to access it later
    /// </summary>
    /// <param name="_result">Async Result</param>
    public static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);      //Restart Listening for new Client
        Console.WriteLine($"Incoming Connection is free {_client.Client.RemoteEndPoint}....");
        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }
        Console.WriteLine($"{_client.Client.RemoteEndPoint} Failed to connect : Server is full!");
    }

    /// <summary>
    /// Since udp have single port from where all the client data is handled Directly the hnadling of data will be done
    /// in this method and sent to respective clients
    /// </summary>
    /// <param name="_result"></param>
    public static void UdpRecieveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndpoint);
            udpListener.BeginReceive(UdpRecieveCallback, null);

            if (_data.Length < 4)
            {
                return;
            }
            using (Packet _packet = new Packet(_data))
            {
                int _clientid = _packet.ReadInt();

                if (_clientid == 0)
                {
                    return;
                }

                if (clients[_clientid].udp.endpoint == null)
                {
                    clients[_clientid].udp.Connect(_clientEndpoint); //If its a dummy packet just to connect                       
                    return;
                }

                if (clients[_clientid].udp.endpoint.ToString() == _clientEndpoint.ToString())
                {
                    clients[_clientid].udp.HandleData(_packet); //If data ther Then handle
                }

            }
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Error in recieing UDP Data:{_ex}");
        }

    }
    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Error in sending UDP data : {_ex}");
        }
    }

    private static void InitialiseServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }
        PacketHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived,ServerHandle.WelcomeRecieved } ,
                {(int)ClientPackets.Command,ServerHandle.CommandHandle },
                {(int)ClientPackets.QuickActions,ServerHandle.QuickActionCommand },
                {(int)ClientPackets.UIAnswer,ServerHandle.UIAnswerCommand },
                {(int)ClientPackets.SendFile,ServerHandle.SendFileCommands },
                {(int)ClientPackets.CloseCommands,ServerHandle.CloseAllOsManager },
                {(int)ClientPackets.QuickActionGamers,ServerHandle.QuickActionGamers }


            };
        Console.WriteLine("Initialized the packets");
    }
    public static void stop()
    {
        tcpListener.Stop();
        udpListener.Close();
    }

}

