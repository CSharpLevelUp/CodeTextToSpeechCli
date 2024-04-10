using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Commands;
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
                    if (command.Verify(ref appStateProxy)) ExecuteCommand(command);
                } catch(CliCommandInvalidException e)
                {
                    Console.Error.WriteLine(e.Message);
                    ExecuteCommand(new HelpCommand());
                }
            } while(appState.HasNextArg);
        }
    }
}
