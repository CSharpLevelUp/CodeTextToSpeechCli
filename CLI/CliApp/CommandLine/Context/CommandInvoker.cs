using System.Diagnostics;
using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Commands;
using CliApp.CommandLine.DataClasses;
using CliApp.CommandLine.Exceptions;

namespace CliApp.CommandLine.Context
{
    public class CommandInvoker
    {
        private AppStateProxy appStateProxy;
        private readonly AppState appState;

        public CommandInvoker(AppState appState)
        {
            this.appState = appState;
            appStateProxy = new AppStateProxy(ref this.appState);
        }
        public void ExecuteCommand(BaseCommand command)
        {
            appState.AddCommandToHistory(command);
            command.Execute(ref appStateProxy);
        }
        public void Run()
        {
            BaseCommand command;
            do
            {
                string CurrentArg = appState.GetArgAndMoveToNext();
                command = appState.GetCommand(CurrentArg);
                try
                {
                    switch(command.Verify(ref appStateProxy))
                    {
                        case CommandVerifyOutcome.Success: 
                            ExecuteCommand(command);
                            break;
                        case CommandVerifyOutcome.Failure:
                            ExecuteCommand(new HelpCommand());
                            break;
                        case CommandVerifyOutcome.SetArguments:
                            SetCommandArgs(command);
                            ExecuteCommand(command);
                            break;
                        default: throw new UnreachableException();
                    };
                } catch(CliCommandInvalidException e)
                {
                    Console.Error.WriteLine(e.Message);
                    ExecuteCommand(new HelpCommand());
                }
            } while(appState.HasNextArg);
        }

        public void SetCommandArgs(BaseCommand command)
        {
            int expectedArgCount = command.ExpectedArgumentsCount; 
            HashSet<string> requiredArgs = command.GetRequiredArgs();
            string commandArg;
            while (command.Arguments is not null)
            {
                commandArg = appState.GetArgAndMoveToNext();
                try
                {
                    if (!commandArg[..2].Equals("--") || commandArg[..2].Length < 1) throw new CliCommandInvalidException($"Invalid argument: {commandArg} arguments must start with --");
                    commandArg = commandArg[2..];
                    var commandArgKeyVal = commandArg.Split("=");
                    if (commandArgKeyVal.Length != 2) throw new CliCommandInvalidException($"Invalid argument: {commandArg}, value missing for key");
                    var commandArgKey = commandArgKeyVal[0].Trim();
                    requiredArgs.Remove(commandArgKey);
                    command.Arguments[commandArgKey].Value = commandArgKeyVal[1].Trim();
                } 
                catch(Exception e) when (e is ArgumentOutOfRangeException || e is CliCommandArgumentNotFoundException)
                {
                    if (e is CliCommandArgumentNotFoundException) throw new CliCommandInvalidException(e.Message);
                    else throw new CliCommandInvalidException($"Invalid argument: {commandArg}");
                }
                expectedArgCount--;
                if (expectedArgCount == 0 || !appState.HasNextArg) break;
            }
            if (requiredArgs.Count > 0) throw new CliCommandInvalidException($"Missing required arguments {requiredArgs}");
        }
    }
}
