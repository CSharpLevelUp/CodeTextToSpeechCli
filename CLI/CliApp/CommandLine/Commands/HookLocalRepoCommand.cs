using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Context;

namespace CliApp.CommandLine.Commands
{
    public class HookLocalRepoCommand: BaseActionCommand
    {
        public override string Name => "hook-local-repo";
        public override string HelpText => "Register File Hook On Local Repo";
        public override Dictionary<string, BaseArgument>? Arguments => new Dictionary<string, BaseArgument>
        {
            { "folder-path", new RequiredArgument() }
        };

        public override void Execute(ref CommandContext Context)
        {
            Console.WriteLine("Adding Hooks");
        }
    }
}
