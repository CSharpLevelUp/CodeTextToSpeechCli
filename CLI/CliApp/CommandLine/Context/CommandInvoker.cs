using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Commands;
using CliApp.CommandLine.Exceptions;

namespace CliApp.CommandLine.Context
{
    public class CommandInvoker
    {
        private readonly List<BaseCommand> CommandHistory = [];
        private CommandContext CmdContext;
        private readonly CliAppContext CliContext;

        public CommandInvoker(CliAppContext cliContext)
        {
            CliContext = cliContext;
            CmdContext = new CommandContext(ref CliContext);
        }
        public void ExecuteCommand(BaseCommand command)
        {
            CommandHistory.Add(command);
            command.Execute(ref CmdContext);
        }

        public void Run()
        {
            BaseCommand command;
            do
            {
                string CurrentArg = CliContext.GetArgAndMoveToNext();
                command = CmdContext.GetCommand(CurrentArg);
                CliContext.PrevCommandInfo = command.GetCommandInfo();
                try
                {
                    if (command.Verify(ref CmdContext)) ExecuteCommand(command);
                } catch(CliCommandInvalidException e)
                {
                    Console.Error.WriteLine(e.Message);
                    ExecuteCommand(new HelpCommand());
                }
            } while(CliContext.HasNextArg);
        }
    }
}
