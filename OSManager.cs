using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

class OSManager
{
    public static List<OSManager> osManagerInstancesRunning = new List<OSManager>();
    private StringBuilder commandOutput = null;
    private int numOutputLines = 0;
    private int ToClient;
    private string Command;
    private Process CMDProcess = new Process();
    private CancellationToken Token;
    


    public void CommandLine(string _command, int _toClient, bool _nirCommand,CancellationToken token)
    {
        ToClient = _toClient;
        Command = _command;
        Token = token;
        CMDProcess.StartInfo.FileName = "cmd.exe";
        CMDProcess.StartInfo.UseShellExecute = false;
        CMDProcess.StartInfo.RedirectStandardOutput = true;
        CMDProcess.StartInfo.CreateNoWindow = false;
        commandOutput = new StringBuilder();
        CMDProcess.OutputDataReceived += OutputHandlerCommandASYNC;
        CMDProcess.StartInfo.RedirectStandardInput = true;
        if (_nirCommand)
            CMDProcess.StartInfo.WorkingDirectory = Constants.nirCommandPath;
        CMDProcess.Start();
        StreamWriter sortStreamWriter = CMDProcess.StandardInput;
        CMDProcess.BeginOutputReadLine();
        sortStreamWriter.WriteLine(_command);
        sortStreamWriter.Close();
        CMDProcess.WaitForExit();
    }
    public void QuickAction(string _command,int _toClient,bool _nirCommand, CancellationToken token)
    {
        Console.WriteLine("Current work Direc: " + System.AppContext.BaseDirectory);
        ToClient = _toClient;
        Command = _command;
        Token = token;
        CMDProcess.StartInfo.FileName = "cmd.exe";
        CMDProcess.StartInfo.UseShellExecute = false;
        CMDProcess.StartInfo.RedirectStandardOutput = true;
        commandOutput = new StringBuilder();
        CMDProcess.StartInfo.RedirectStandardInput = true;
        if (_nirCommand)
            CMDProcess.StartInfo.WorkingDirectory = Constants.nirCommandPath;
        CMDProcess.Start();
        StreamWriter sortStreamWriter = CMDProcess.StandardInput;
        sortStreamWriter.WriteLine(_command);
        sortStreamWriter.Close();
        CMDProcess.WaitForExit();
        CMDProcess.Close();
    }
    public void UIAnswer(string _command,int _toClient,bool _nirCommand, CancellationToken token)
    {
        ToClient = _toClient;
        Command = _command;
        Token = token;
        CMDProcess.StartInfo.FileName = "cmd.exe";
        CMDProcess.StartInfo.UseShellExecute = false;
        CMDProcess.StartInfo.RedirectStandardOutput = true;
        commandOutput = new StringBuilder();
        CMDProcess.OutputDataReceived += OutputHandlerUIAnswerAsync;
        CMDProcess.StartInfo.RedirectStandardInput = true;
        if (_nirCommand)
            CMDProcess.StartInfo.WorkingDirectory = Constants.nirCommandPath;
        CMDProcess.Start();
        StreamWriter sortStreamWriter = CMDProcess.StandardInput;
        CMDProcess.BeginOutputReadLine();
        sortStreamWriter.WriteLine(_command);
        sortStreamWriter.Close();
        CMDProcess.WaitForExit();
        CMDProcess.Close();
    }
    public void QuickActionGamers(string _command,int _toClient,bool _nirCommand, CancellationToken token)
    {
        ToClient = _toClient;
        Command = _command;
        Token = token;
        CMDProcess.StartInfo.FileName = "cmd.exe";
        CMDProcess.StartInfo.UseShellExecute = false;
        CMDProcess.StartInfo.RedirectStandardOutput = true;
        commandOutput = new StringBuilder();
        CMDProcess.OutputDataReceived += OutputHandlerQuickActionGamersAsync;
        CMDProcess.StartInfo.RedirectStandardInput = true;
        if (_nirCommand)
            CMDProcess.StartInfo.WorkingDirectory = Constants.nirCommandPath;
        CMDProcess.Start();
        StreamWriter sortStreamWriter = CMDProcess.StandardInput;
        CMDProcess.BeginOutputReadLine();
        sortStreamWriter.WriteLine(_command);
        sortStreamWriter.Close();
        CMDProcess.WaitForExit();
        CMDProcess.Close();
    }
    

    private void OutputHandlerCommandASYNC(object sendingProcess,DataReceivedEventArgs outLine)
    {
        if(Token.IsCancellationRequested)
        {
            Console.WriteLine("Called Cancellation");
            CMDProcess.CancelOutputRead();
        }
        if (!String.IsNullOrEmpty(outLine.Data))
        {
            numOutputLines++;
            commandOutput.Remove(0,commandOutput.Length);
            commandOutput.Append(outLine.Data);
            ServerSend.sendCommandResult(ToClient, Command, commandOutput.ToString());

        }
    }
    private void OutputHandlerUIAnswerAsync(object sendingProcess,DataReceivedEventArgs outLine)
    {
        if (Token.IsCancellationRequested)
        {
            Console.WriteLine("Called Cancellation");
            CMDProcess.CancelOutputRead();
        }
        if (!String.IsNullOrEmpty(outLine.Data))
        {
            numOutputLines++;
            commandOutput.Remove(0, commandOutput.Length);
            commandOutput.Append(outLine.Data);
            ServerSend.sendUIAnswer(ToClient, Command, commandOutput.ToString());

        }
    }
    private void OutputHandlerQuickActionGamersAsync(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (Token.IsCancellationRequested)
        {
            Console.WriteLine("Called Cancellation");
            CMDProcess.CancelOutputRead();
        }
        if (!String.IsNullOrEmpty(outLine.Data))
        {
            numOutputLines++;
            commandOutput.Remove(0, commandOutput.Length);
            commandOutput.Append(outLine.Data);
            throw new NotImplementedException();
           // ServerSend.sendCommandResultLine(ToClient, Command, commandOutput.ToString());

        }
    }

    public void dispose()
    {
        CMDProcess.OutputDataReceived -= OutputHandlerCommandASYNC;
        osManagerInstancesRunning.Remove(this);
    }
}




