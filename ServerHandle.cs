using System.Collections;
using System.Collections.Generic;
using System;
public class ServerHandle
{

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
    public static void SingleLineCommand(int _fromClient, Packet _packet)
    {
        int clientIdCheck = _packet.ReadInt();
        string username = _packet.ReadString();
        string password = _packet.ReadString();
        bool nircmd = _packet.ReadBool();
        string command = _packet.ReadString();
        Console.WriteLine("Handling a single Line command: " + command);
        if (clientIdCheck == _fromClient)
        {
            if (DataAboutUser.UsnPwd.ContainsKey(username))
            {
                if (DataAboutUser.UsnPwd[username] == password)
                {
                    OSManager.instance.RunSingleCommand(command, _fromClient, nircmd);
                }
                else
                    Console.WriteLine($"The client password not matching");

            }
            else
                Console.WriteLine($"The client username not matching");

        }
        else
            Console.WriteLine($"The client id is not matching");


    }
    public static void MultiLineCommands(int _fromClient, Packet _packet)
    {
        int clientIdCheck = _packet.ReadInt();
        string username = _packet.ReadString();
        string password = _packet.ReadString();
        bool nircmd = _packet.ReadBool();
        List<string> commandList = _packet.ReadList();
        Console.WriteLine("Handling the Multiline Command of Lenght : " + commandList.Count);
        if (clientIdCheck == _fromClient)
        {
            if (DataAboutUser.UsnPwd.ContainsKey(username))
            {
                if (DataAboutUser.UsnPwd[username] == password)
                {
                    OSManager.instance.RunMultilineCommands(commandList, _fromClient, nircmd);
                }
                else
                    Console.WriteLine($"The client password not matching");

            }
            else
                Console.WriteLine($"The client username not matching");

        }
        else
            Console.WriteLine($"The client id is not matching");


    }
    public static void SendFileCommands(int _fromClient, Packet packet)
    {
        ServerSend.SendFile(_fromClient, packet.ReadString());
    }


}
