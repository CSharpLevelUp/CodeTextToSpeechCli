using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Context;

namespace CliApp.CommandLine.Commands
{
    public class HookLocalRepoCommand: BaseActionCommand
    {
        public override string Name => "hook-local-repo";
        public override string HelpText => "Registers Git Hooks on local repo";
        public override Dictionary<string, BaseArgument>? Arguments => new Dictionary<string, BaseArgument>
        {
            { "folder-path", new RequiredArgument("Local path to the repo directory") }
        };

        public override void Execute(ref AppStateProxy appProxy)
        {
            Console.WriteLine("Adding Hooks");
        }
    }
}
