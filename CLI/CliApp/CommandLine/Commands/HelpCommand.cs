using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Context;

namespace CliApp.CommandLine.Commands
{
    public class HelpCommand: BaseActionCommand
    {
        public override string Name => "help";
        public override string HelpText => "Help Command Text";
        public override Dictionary<string, BaseArgument>? Arguments => null;
        public override void Execute(ref AppStateProxy appProxy)
        {
            if (appProxy.PreviousRanCommand == null || appProxy.PreviousRanCommand.Equals(GetCommandInfo())) 
            {
                foreach (var commandName in appProxy.GetAvailableCommandNames())
                {
                    BaseCommand command = appProxy.GetCommand(commandName);
                    Console.WriteLine(command.HelpText);
                }
            } else {
                Console.WriteLine(appProxy.PreviousRanCommand.HelpText);
            }
        }
    }
}
