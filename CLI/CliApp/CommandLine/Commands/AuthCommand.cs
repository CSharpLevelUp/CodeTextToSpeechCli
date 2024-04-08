using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Context;

namespace CliApp.CommandLine.Commands
{
    public class AuthCommand: BaseActionCommand
    {
        public override string Name => "auth";
        public override string HelpText => "Auth Command Help";

        public override Dictionary<string, BaseArgument>? Arguments => null;

        public override void Execute(ref CommandContext context)
        {
            Console.WriteLine("Executing Auth Command");
        }
    }
}
