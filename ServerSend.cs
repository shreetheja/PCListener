using System.Collections;
using System.Collections.Generic;
using System;
public class ServerSend
{
    #region TCP Send to Client Methods
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
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
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }


    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
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
    /// <summary>
    /// Sends a Welcome Message with a Request of password and username
    /// </summary>
    /// <param name="_toClient">To which client to send</param>
    /// <param name="_msg"Message to be sent></param>
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

    /// <summary>
    /// To denote the invalid Login Password or username wrong
    /// </summary>
    /// <param name="_toClient"></param>
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

    /// <summary>
    /// Denots login suceccews
    /// </summary>
    /// <param name="_toClient"></param>
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

    /// <summary>
    /// Sending UI Answer Result
    /// </summary>
    /// <param name="_toClient"></param>
    /// <param name="Command">Command which is stored in form of list shud be sent in list form to match the type in client side</param>
    /// <param name="res">Result line </param>
    public static void sendUIAnswer(int _toClient, string Command, string res,bool isFinal = false)  
    {
        using (Packet _packet = new Packet((int)ServerPackets.UIAnswer))
        {
            _packet.Write(isFinal);
            _packet.Write(Command);
            _packet.Write(res);
            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>
    /// Sending the Result Line By line Executed by a asynchronous process
    /// </summary>
    /// <param name="_toClient"></param>
    /// <param name="Command"></param>
    /// <param name="Finalres"></param>
    public static void sendCommandResult(int _toClient, string Command, string Finalres)
    {
        using (Packet _packet = new Packet((int)ServerPackets.CommandResult))
        {
            _packet.Write(Command);
            _packet.Write(Finalres);
            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>
    /// Sending the Result Line By line asynchronously this is end of command 
    /// </summary>
    /// <param name="_toClient"></param>
    /// <param name="Command"></param>
    /// <param name="Finalres"></param>
    public static void sendEndOfCommandResult(int _toClient, string Command, string Finalres)
    {
        Console.WriteLine("Sending Final Signal Line ...."+Command);
        using (Packet _packet = new Packet((int)ServerPackets.CommandFinalResult))
        {
            _packet.Write(Command);
            _packet.Write(Finalres);
            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>
    /// Sending the Result Line By line Executed by a asynchronous process of quick action
    /// </summary>
    /// <param name="_toClient"></param>
    /// <param name="Command"></param>
    /// <param name="Finalres"></param>
    public static void sendQuickGamersCommandResult(int _toClient, string Command,string result,bool isFinalRes = false)
    {
        using (Packet _packet = new Packet((int)ServerPackets.QuickActionGamers))
        {
            _packet.Write(Command);
            _packet.Write(result);
            _packet.Write(isFinalRes);
            SendTCPData(_toClient, _packet);
        }
    }



    /// <summary>
    /// </summary>
    /// <param name="_toClient"></param>
    /// <param name="path"></param>
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

