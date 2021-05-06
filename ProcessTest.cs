using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;

class ProcessTest
{
    // Define static variables shared by class methods.
    private static StringBuilder sortOutput = null;
    private static int numOutputLines = 0;

    public async static Task CommandLine(string _command,int _toClient,bool _nirCommand)
    {
        // Initialize the process and its StartInfo properties.
        // The sort command is a console application that
        // reads and sorts text input.

        Process sortProcess = new Process();
        sortProcess.StartInfo.FileName = "cmd.exe";

        // Set UseShellExecute to false for redirection.
        sortProcess.StartInfo.UseShellExecute = false;

        // Redirect the standard output of the sort command.
        // This stream is read asynchronously using an event handler.
        sortProcess.StartInfo.RedirectStandardOutput = true;
        sortOutput = new StringBuilder();

        // Set our event handler to asynchronously read the sort output.
        sortProcess.OutputDataReceived += SortOutputHandler;

        // Redirect standard input as well.  This stream
        // is used synchronously.
        sortProcess.StartInfo.RedirectStandardInput = true;

        // Start the process.
        sortProcess.Start();

        // Use a stream writer to synchronously write the sort input.
        StreamWriter sortStreamWriter = sortProcess.StandardInput;

        // Start the asynchronous read of the sort output stream.
        sortProcess.BeginOutputReadLine();

        //write command to stream
        sortStreamWriter.WriteLine(_command);

        // End the input stream to the sort command.
        sortStreamWriter.Close();

        // Wait for the sort process to write the sorted text lines.
        sortProcess.WaitForExit();

        sortProcess.Close();
        
    }

    private static void SortOutputHandler(object sendingProcess,
        DataReceivedEventArgs outLine)
    {
        // Collect the sort command output.
        if (!String.IsNullOrEmpty(outLine.Data))
        {
            numOutputLines++;

            // Add the text to the collected output.
            sortOutput.Append(Environment.NewLine +
                $"[{numOutputLines}] - {outLine.Data}");
            Console.WriteLine(outLine.Data);

        }
    }
}




