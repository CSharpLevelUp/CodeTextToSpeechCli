using System.Diagnostics;
using System.Text;

namespace Shared;

public class ProcessRunner: IDisposable
{
    public bool HasError
    {
        get => stdErrBuilder.Length > 0;
    }
    private StringBuilder stdOutBuilder;
    private StringBuilder stdErrBuilder;
    private readonly Process process;
    private bool appendNewline = false;

    public ProcessRunner(string FileName, string WorkingDirectory = "")
    {
        stdOutBuilder = new StringBuilder(); 
        stdErrBuilder = new StringBuilder();

        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = FileName,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = WorkingDirectory
            }
        };
        process.OutputDataReceived += StdOutHandler;
        process.ErrorDataReceived += StdErrHandler;
    }

    public string RunCommand(string commandOpts, bool appendNewline=false)
    {
        this.appendNewline = appendNewline;
        ClearOutputs();
        process.StartInfo.Arguments = commandOpts;
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        process.Close();
        this.appendNewline = false;
        if (HasError) throw new ProcessRunnerException(stdErrBuilder.ToString());
        return stdOutBuilder.ToString().Trim();
    }

    private void ClearOutputs()
    {
        stdErrBuilder.Clear();
        stdOutBuilder.Clear();
    }

    private void StdOutHandler(object sendingProcess, DataReceivedEventArgs line)
    {
        RightToStringBuilder(stdOutBuilder, line.Data);
    }

    private void StdErrHandler(object sendingProcess, DataReceivedEventArgs line)
    {
        RightToStringBuilder(stdErrBuilder, line.Data);
    }

    private void RightToStringBuilder(StringBuilder stringBuilder, string? data)
    {
        if(!String.IsNullOrEmpty(data)) stringBuilder.Append($"{data}{ (appendNewline? '\n' : "")}");
    }

    public void Dispose()
    {
        // So any derived class doesn't need to implement thier own Dispose method
        GC.SuppressFinalize(this);
        process.Dispose();
    }
}


public class ProcessRunnerException: Exception
{
    public ProcessRunnerException(string message): base(message)
    {
    }
}