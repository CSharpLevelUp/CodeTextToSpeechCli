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
                foreach (var commandName in appProxy.GetAvailableCommandNames()) PrintHelpText(appProxy.GetCommand(commandName));
            } else {
                PrintHelpText(this);
            }
        }

        private void PrintHelpText(BaseCommand command)
        {
            Console.WriteLine("COMMAND NAME:");
            Console.WriteLine("=============");
            Console.WriteLine($"{command.Name}");
            Console.WriteLine();
            if (!string.IsNullOrEmpty(command.HelpText))
            {
                Console.WriteLine("DESCRIPTION:");
                Console.WriteLine("============");
                Console.WriteLine($"{command.HelpText}");
                Console.WriteLine();
            }
            if (command.Arguments is not null)
            {
                Console.WriteLine("ARGUMENTS:");
                Console.WriteLine("==========");
                foreach (var argumentName in command.Arguments.Keys)
                {
                    Console.WriteLine($"--{argumentName}=<value>");
                    Console.WriteLine();
                    Console.WriteLine($"{command.Arguments[argumentName].Description}");
                    Console.WriteLine();
                }
            }
        }
    }
}
