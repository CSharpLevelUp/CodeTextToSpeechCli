using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Context;

namespace CliApp.CommandLine.Commands
{
    public class HelpCommand: BaseActionCommand
    {
        public override string Name => "help";
        public override string HelpText => "Help Command Text";
        public override Dictionary<string, BaseArgument>? Arguments => null;
        public override void Execute(ref CommandContext context)
        {
            if (context.PreviousRanCommand == null || context.PreviousRanCommand.Equals(GetCommandInfo())) 
            {
                foreach (var commandName in context.GetAvailableCommandNames())
                {
                    BaseCommand command = context.GetCommand(commandName);
                    Console.WriteLine(command.HelpText);
                }
            } else {
                Console.WriteLine(context.PreviousRanCommand.HelpText);
            }
        }
    }
}
