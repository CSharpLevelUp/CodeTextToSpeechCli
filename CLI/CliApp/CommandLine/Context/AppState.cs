using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Commands;
using CliApp.CommandLine.Config;
using CliApp.CommandLine.DataClasses;
using CliApp.CommandLine.Exceptions;

namespace CliApp.CommandLine.Context
{
    public class AppState
    {
        private CommandInfo? _prevCommandInfo;
        public CommandInfo? PrevCommandInfo => _prevCommandInfo;
        private BaseAvailableCommands _availableCommands;
        public BaseAvailableCommands AvailableCommands
        {
            get 
            {
                return _availableCommands;
            }
            set
            {
                AvailableCommandsHistory.Add(value);
                _availableCommands = value;
            }
        }

        public readonly string[]? Args;
        private int CurrentArgIdx = 0;

        private readonly List<BaseAvailableCommands?> AvailableCommandsHistory = [];
        private readonly List<BaseCommand> CommandHistory = [];
        public void AddCommandToHistory(BaseCommand command)
        {
            CommandHistory.Add(command);
            _prevCommandInfo = command.GetCommandInfo();
        }
        public AppState(string[] args)
        {
            _availableCommands = new InitialAvailableCommands();
            AvailableCommandsHistory.Add(_availableCommands);
            Args = args;
        }

        public IEnumerable<string> GetAvailableCommandNames()
        {
            return AvailableCommands.CommandsMap.Keys;
        }

        public BaseCommand GetCommand(string name)
        {
            try 
            {
                return AvailableCommands.CommandsMap[name];
            } catch(KeyNotFoundException)
            {
                throw new CliCommandNotFoundException(name);
            }
        }

        public string GetCurrentArg()
        {
            if (Args is not null) return (Args.Length == 0) ? "help" : Args[CurrentArgIdx];
            else throw new Exception("Args array is null");
        }

        public string GetArgAndMoveToNext()
        {
            if (HasNextArg)
            {
                var arg = GetCurrentArg(); 
                CurrentArgIdx++;
                return arg;
            } else if(CurrentArgIdx == 0) return "help";
            else throw new CliCommandException("Exhausted unprocessed args");
        }

        public bool HasNextArg => Args != null && CurrentArgIdx < Args.Length;

        public string? GetNextArgWithoutMovingIdx()
        {
            if (!HasNextArg) return null;
            return Args?[CurrentArgIdx + 1];
        }

        public string? GetArgWithoutMovingIdx()
        {
            if (Args?.Length > 0) return Args[CurrentArgIdx];
            return null;
        }
    }
}
