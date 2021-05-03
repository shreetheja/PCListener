using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using System.Threading.Tasks;
public class OSManager
{
    public delegate void SingleCommandHandler(string command, int toClient, bool nirCommand = false);
    public delegate void MultiCommandHandler(List<string> command, int toClient, bool nirCommand = false);
    public SingleCommandHandler SingleLine;
    public MultiCommandHandler MultiLine;
    public bool Executing = false;
    public int NewCommand = 0;
    public List<string> lastOut = new List<string>();
    string NIRpath = "C:\\Users\\snsha\\Desktop\\NETWORK\\Server\\Assets\\nircmd-x64";
    string lastBuffer;
    private void Start()
    {
        // SingleLine = new SingleCommandHandler(IRunSingleCommand);
        //MultiLine = new MultiCommandHandler(IRunMultilineCommands);
    }
    private void LateUpdate()
    {
        if (NewCommand < 0)
        {
            NewCommand = 0;
        }
    }//making the execution safe
    public async Task RunSingleCommand(string command, int toClient, bool nirCommand = false, float timeOfExec = 1)
    {
        NewCommand++;
        await IRunSingleCommand(command, toClient, nirCommand);
    }
    public async Task RunMultilineCommands(List<string> commands, int toClient, bool nirCommand = false)
    {
        NewCommand++;
        Executing = false;
        await IRunMultilineCommands(commands, toClient, nirCommand);
    }
    private async Task IRunSingleCommand(string command, int toClient, bool nirCommand)
    {
        Console.WriteLine("Executing : " + command);
        Executing = true;
        NewCommand--;
        StreamWriter sw;
        StreamReader sr;
        StreamReader err;
        string Line;
        lastBuffer = "started";
        using (Process pr = new Process())
        {
            ProcessStartInfo pri = new ProcessStartInfo("cmd.exe")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            if (nirCommand)
                pri.WorkingDirectory = NIRpath;
            pr.StartInfo = pri;
            pr.Start();
            sr = pr.StandardOutput;
            sw = pr.StandardInput;
            err = pr.StandardError;
            pr.StandardInput.WriteLine(command);
            while ((Line = sr.ReadLine()) != null)
            {
                ServerSend.sendCommandResultLine(toClient, command, Line);

            }
            Console.WriteLine("SendingFinal Command");
            ServerSend.sendEndOfCommandResult(toClient, command, lastBuffer);
            pr.Close();
        }

    }

    internal void UIAnswer(List<string> command, int fromClient, bool nircmd)
    {
        Console.WriteLine("Executing UIAnswer Command");
    }

    internal void RunQuickAction(List<string> command, int fromClient, bool nircmd)
    {
        Console.WriteLine("Executing Quick Command");
    }

    private async Task IRunMultilineCommands(List<string> commands, int _toClient, bool nirCommand = false)
    {
        Console.WriteLine("Executing : " + commands.Count);
        Executing = true;
        NewCommand--;
        StreamWriter sw;
        StreamReader sr;
        StreamReader err;
        string Line;
        lastBuffer = "started";
        using (Process pr = new Process())
        {
            ProcessStartInfo pri = new ProcessStartInfo("cmd.exe")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            if (nirCommand)
                pri.WorkingDirectory = NIRpath;
            pr.StartInfo = pri;
            pr.Start();
            sr = pr.StandardOutput;
            sw = pr.StandardInput;
            err = pr.StandardError;
            foreach(string command in commands)
            pr.StandardInput.WriteLine(command);
            while ((Line = sr.ReadLine()) != null)
            {
                Console.WriteLine("ReadLine: " + Line);
                ServerSend.sendCommandResultLine(_toClient, String.Join("\n", commands), Line);
            }
            Console.WriteLine("SendingFinal Command");
            ServerSend.sendEndOfCommandResult(_toClient, String.Join("\n", commands), lastBuffer); ;
            pr.Close();
        }
    }
    private void endSingleline()
    {
        Executing = false;
        Console.WriteLine("CameOutSingle");

    }
    private void endMultiline(IAsyncResult t)
    {
        Executing = false;
        Console.WriteLine("CameOutMulti");

    }
}