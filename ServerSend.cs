using System.Collections;
using System.Collections.Generic;
using System;
public class ServerSend
{
    #region TCP Send to Client Methods
    public static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }
    public static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    public static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }
    #endregion

    #region UDP Send to Client Methods
    public static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }


    public static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    public static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }
    #endregion
    #region Types of Packets
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
        Console.WriteLine("Sent Request of username and passoword");

    }
    public static void InvalidLoginId(int _toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.invalidLogin))
        {
            _packet.Write("The Sent Password and username is wrong please try again");
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
        Console.WriteLine("Sent Request of username and passoword login was invalid");
    }
    public static void LoginSuccess(int _toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.loginSuccess))
        {
            _packet.Write("The Sent Password and username was correct");
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
        Console.WriteLine("Sent Request of success");
    }
    public static void sendCommandResults(int _toClient, List<string> res)
    {
        Console.WriteLine("Sending Command Results : " + res);
        using (Packet _packet = new Packet((int)ServerPackets.CommandResult))
        {
            _packet.Write(res.Count);
            _packet.Write(res);

            SendTCPData(_toClient, _packet);
        }

    }
    public static void sendCommandResultLine(int _toClient, string res)
    {
        Console.WriteLine("Sending Result Line ....");
        using (Packet _packet = new Packet((int)ServerPackets.CommandResult))
        {
            _packet.Write(res);
            SendTCPData(_toClient, _packet);
        }
    }
    public static void sendEndOfCommandResult(int _toClient, string Finalres)
    {
        Console.WriteLine("Sending Final Signal Line ....");
        using (Packet _packet = new Packet((int)ServerPackets.CommandFinalResult))
        {
            _packet.Write(Finalres);
            SendTCPData(_toClient, _packet);
        }
    }
    public static void SendFile(int _toClient, string path)
    {
        using (Packet _packet = new Packet((int)ServerPackets.SentFile))
        {
            string[] st = path.Split('\\');
            string filename = st[st.Length - 1];
            _packet.Write(path, filename);
            SendTCPData(_toClient, _packet);
        }
    }

    #endregion
}

