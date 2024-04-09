using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Commands;
using CliApp.CommandLine.Config;
using CliApp.CommandLine.DataClasses;
using CliApp.CommandLine.Exceptions;

namespace CliApp.CommandLine.Context
{
    public class CliAppContext
    {
        private CommandInfo? _prevCommandInfo;
        public CommandInfo? PrevCommandInfo
        {
            get
            {
                return _prevCommandInfo;
            }
            set
            {
                _prevCommandInfo = value;
            }
        }

        public BaseAvailableCommands AvailableCommands
        {
            get 
            {
                return _stateCommands;
            }
            set
            {
                AvailableCommandsHistory.Add(value);
                _stateCommands = value;
            }
        }

        public readonly string[]? Args;
        private int CurrentArgIdx = 0;

        private BaseAvailableCommands _stateCommands;

        private readonly List<BaseAvailableCommands?> AvailableCommandsHistory = [];
        public CliAppContext(string[] args)
        {
            _stateCommands = new InitialAvailableCommands();
            AvailableCommandsHistory.Add(_stateCommands);
            Args = args;
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
