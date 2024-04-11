using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Commands;
using CliApp.CommandLine.DataClasses;

namespace CliApp.CommandLine.Config
{
    public class InitialAvailableCommands: BaseAvailableCommands
    {
        public override BaseCommand[] AvailableCommands => [
            new HookLocalRepoCommand(),
            new SetOpenAiApiKeyCommand()
        ];

        public InitialAvailableCommands(): base()
        {

        }
    }
}
