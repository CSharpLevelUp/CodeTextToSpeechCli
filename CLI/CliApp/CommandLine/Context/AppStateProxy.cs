using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.DataClasses;
using CliApp.CommandLine.Exceptions;

namespace CliApp.CommandLine.Context
{
    public class AppStateProxy(ref AppState appstate)
    {
        private readonly AppState appState = appstate;

        public IEnumerable<string> GetAvailableCommandNames()
        {
            return appState.GetAvailableCommandNames();
        }
        public void SetAvailableCommands(BaseAvailableCommands newAvailableCommands, BaseCommand command)
        {
            if (command.Type is CommandType.Action) throw new CliCommandException("Only Namespace commands can set available commands");
            appState.AvailableCommands = newAvailableCommands;
        }

        public CommandInfo? PreviousRanCommand => appState.PrevCommandInfo;

        public BaseCommand GetCommand(string name)
        {
            return appState.GetCommand(name);
        }

        public string? PeekNextArg => appState.GetArgWithoutMovingIdx();
        public bool HasNextArg => appState.HasNextArg;
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
                    command.SetArgumentValue(commandArgKey, commandArgKeyVal[1].Trim());
                } 
                catch(Exception e) when (e is ArgumentOutOfRangeException || e is CliCommandArgumentNotFoundException)
                {
                    if (e is CliCommandArgumentNotFoundException) throw new CliCommandInvalidException(e.Message);
                    else throw new CliCommandInvalidException($"Invalid argument: {commandArg}");
                }
                expectedArgCount--;
                if (expectedArgCount == 0 || !HasNextArg) break;
            }
            if (requiredArgs.Count > 0) throw new CliCommandInvalidException($"Missing required arguments {requiredArgs}");
        }
    }
}