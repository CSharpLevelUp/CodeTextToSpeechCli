using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Commands;

namespace CliApp.CommandLine.Config
{

    public static class GlobalCommands
    {
        public readonly static BaseCommand[] Commands = [
            new HelpCommand()
        ];
    }
}