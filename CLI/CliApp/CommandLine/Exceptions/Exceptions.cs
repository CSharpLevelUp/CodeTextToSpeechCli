namespace CliApp.CommandLine.Exceptions
{
    public class CliCommandException(string message): Exception(message)
    {
    }

    public class CliCommandNotFoundException(string commandName) : Exception($"Command: {commandName} not found")
    {
    }

    public class CliCommandRequiredArgumentException(string argument): Exception($"Argument: {argument} is required")
    {
    }

    public class CliCommandArgumentNotFoundException(string argument): Exception($"No argument with name: {argument}")
    {
    }

    public class CliCommandInvalidException(string message): Exception(message)
    {
    }
}
