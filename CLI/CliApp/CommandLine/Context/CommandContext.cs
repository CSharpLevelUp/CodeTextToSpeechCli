using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.DataClasses;
using CliApp.CommandLine.Exceptions;

namespace CliApp.CommandLine.Context
{
    public class CommandContext(ref CliAppContext appContext)
    {
        private readonly CliAppContext AppContext = appContext;

        public IEnumerable<string> GetAvailableCommandNames()
        {
            return AppContext.AvailableCommands.CommandsMap.Keys;
        }
        public void SetAvailableCommands(BaseAvailableCommands newAvailableCommands, BaseCommand command)
        {
            if (command.Type is CommandType.Action) throw new CliCommandException("Only Namespace commands can set available commands");
            AppContext.AvailableCommands = newAvailableCommands;
        }

        public CommandInfo? PreviousRanCommand => AppContext.PrevCommandInfo;

        public BaseCommand GetCommand(string name)
        {
            try 
            {
                return AppContext.AvailableCommands.CommandsMap[name];
            } catch(KeyNotFoundException)
            {
                throw new CliCommandNotFoundException(name);
            }
        }

        public string? PeekNextArg => AppContext.GetArgWithoutMovingIdx();
        public bool HasNextArg => AppContext.HasNextArg;

        public void SetCommandArgs(BaseCommand command)
        {
            int expectedArgCount = command.ExpectedArgumentsCount; 
            HashSet<string> requiredArgs = command.GetRequiredArgs();
            string commandArg;
            while (HasNextArg && command.Arguments is not null)
            {
                commandArg = AppContext.GetArgAndMoveToNext();
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
                if (expectedArgCount == 0) break;
            }
            if (requiredArgs.Count > 0) throw new CliCommandInvalidException($"Missing required arguments {requiredArgs}");
        }
    }
}