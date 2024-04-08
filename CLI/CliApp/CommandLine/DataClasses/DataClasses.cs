using CliApp.CommandLine.CommandBase;

namespace CliApp.CommandLine.DataClasses
{
    public class CommandInfo(BaseCommand command)
    {
        public readonly CommandType Type = command.Type;
        public readonly string HelpText = command.HelpText;

        public readonly string CommandName = command.Name;
    }

    public enum CommandType
    {
        NameSpace = 0,
        Action = 1
    }
}