using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Threading;

public class ServerHandle
{
    public static List<CancellationTokenSource> tokens = new List<CancellationTokenSource>(); 

    #region WelcomeHandshake
    /// <summary>
    /// Handles First Handshake
    /// </summary>
    /// <param name="_fromClient"></param>
    /// <param name="_packet"></param>
    public static void WelcomeRecieved(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();
        string _password = _packet.ReadString();


        if (DataAboutUser.UsnPwd.ContainsKey(_username) && DataAboutUser.UsnPwd[_username] == _password)
        {
            Server.clients[_fromClient].username = _username;
            Server.clients[_fromClient].UsedPassword = _password;
            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} Connected Succesfully and the Player is {_fromClient}.");
            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} Connected Succesfully and the Player password is {_password}.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"The Client Player {_username} has claimed a wrong client ID {_fromClient}");
            }
            ServerSend.LoginSuccess(_fromClient);

        }
        else
        {
            ServerSend.InvalidLoginId(_fromClient);
            Server.clients[_fromClient].disconnect();
        }
    }
    #endregion

    #region CommandHandle
    public async static void CommandHandle(int _fromClient, Packet _packet)
    {
        if(CheckPacketValidity(_fromClient,_packet))
        {
            bool nirCmd = _packet.ReadBool();
            string command = _packet.ReadString();
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            tokens.Add(source);
            await Task.Run(() =>
            CommandTask(_fromClient,command,nirCmd,token),token);
            ServerSend.sendEndOfCommandResult(_fromClient, command, "\nEnd OF Running of these Commands .. ");
        }
        
    }
    private static void CommandTask(int _fromClient,string command,bool nircmd,CancellationToken Token)
    {
            OSManager instance = new OSManager();
            OSManager.osManagerInstancesRunning.Add(instance);
            string[] commands = command.Split('\n');
            if (commands.Length > 1)
            {
                Console.WriteLine("RUNSINGLELINE : " + command);
                instance.CommandLine(command, _fromClient, nircmd,Token);
            }
            else
            {
                Console.WriteLine("RUNMULTI : " + command);
                instance.CommandLine(command, _fromClient, nircmd,Token);
            }
        
    }
    #endregion

    #region QuickActionHandle
    public async static void QuickActionCommand(int _fromClient, Packet _packet)
    {
        if (CheckPacketValidity(_fromClient, _packet))
        {
            bool nircmd = _packet.ReadBool();
            string command = _packet.ReadString();
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            tokens.Add(source);
            await Task.Run(() =>QuickActionTask(_fromClient,command,nircmd,token),token);
        }
        
    }
    private static void QuickActionTask(int _fromClient,string Command,bool nircmd,CancellationToken token)
    {
        OSManager instance = new OSManager();
        string[] commands = Command.Split('\n');
        OSManager.osManagerInstancesRunning.Add(instance);
        if (commands.Length > 1)
        {
            instance.QuickAction(Command, _fromClient, nircmd,token);
        }
        else
        {
            instance.QuickAction(Command, _fromClient, nircmd,token);
        }
    }
    #endregion

    #region UIAnswer
    public async static void UIAnswerCommand(int _fromClient, Packet _packet)
    {
        
        if (CheckPacketValidity(_fromClient, _packet))
        {
            Console.WriteLine("Recieved UI Command");
            bool nircmd = _packet.ReadBool();
            string command = _packet.ReadString();
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            tokens.Add(source);
            await Task.Run(() => UIAnswerTask(_fromClient, command, nircmd,token),token);
        }
    }
    private static void UIAnswerTask(int _fromClient,string Command,bool nircmd,CancellationToken token)
    {
        OSManager instance = new OSManager();
        string[] commands = Command.Split('\n');
        OSManager.osManagerInstancesRunning.Add(instance);
        if (commands.Length > 1)
        {
            instance.UIAnswer(Command, _fromClient, nircmd,token);
        }
        else
        {
            instance.UIAnswer(Command, _fromClient, nircmd,token);
        }
    }
    #endregion
    
    #region QuickActionGamers
    public async static void QuickActionGamers(int _fromClient, Packet _packet)
    {
        if (CheckPacketValidity(_fromClient, _packet))
        {
            bool nircmd = _packet.ReadBool();
            string command = _packet.ReadString();
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            tokens.Add(source);
            await Task.Run(() => QuickActionGamersTask(_fromClient, command, nircmd,token),token);
        }
    }
    private static void QuickActionGamersTask(int _fromClient, string Command, bool nircmd,CancellationToken token)
    {
        OSManager instance = new OSManager();
        string[] commands = Command.Split('\n');
        OSManager.osManagerInstancesRunning.Add(instance);
        if (commands.Length > 1)
        {
            instance.QuickActionGamers(Command, _fromClient, nircmd,token);
        }
        else
        {
            instance.QuickActionGamers(Command, _fromClient, nircmd,token);
        }
    }
    #endregion

    #region EXPTFileSend
    public static void SendFileCommands(int _fromClient, Packet packet)
    {
        ServerSend.SendFile(_fromClient, packet.ReadString());
    }
    private static void SendFileTask()
    {

    }
    #endregion

    #region CLOSEALl
    public static void CloseAllOsManager(int _fromClient,Packet Packet)
    {
        if (CheckPacketValidity(_fromClient, Packet))
        {
            foreach(CancellationTokenSource Source in tokens)
            {
                Source.Cancel();
            }
            tokens.Clear();
                
        }
    }
    #endregion

    #region HelperMethods
    private static bool CheckPacketValidity(int _fromClient, Packet _packet)
    {
        int clientIdCheck = _packet.ReadInt();
        string username = _packet.ReadString();
        string password = _packet.ReadString();
        if (clientIdCheck == _fromClient)
        {
            if (DataAboutUser.UsnPwd.ContainsKey(username))
            {
                if (DataAboutUser.UsnPwd[username] == password)
                {
                    return true;
                }
                else
                    Console.WriteLine($"The client password not matching");

            }
            else
                Console.WriteLine($"The client username not matching");

        }
        else
            Console.WriteLine($"The client id is not matching");
        return false;
    }
    #endregion
}
