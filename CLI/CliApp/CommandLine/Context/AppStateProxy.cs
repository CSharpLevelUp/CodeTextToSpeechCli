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
        
    }
}