using CliApp.CommandLine.Context;
using CliApp.CommandLine.DataClasses;
using CliApp.CommandLine.Exceptions;

namespace CliApp.CommandLine.CommandBase
{
    public abstract class BaseCommand
    {
        public abstract CommandType Type { get; } 
        public abstract string HelpText { get; }
        public abstract string Name {get;}

        public abstract void Execute(ref AppStateProxy appProxy);

        public abstract bool Verify(ref AppStateProxy appProxy);
        public abstract Dictionary<string, BaseArgument>? Arguments {get;}

        public CommandInfo GetCommandInfo()
        {
            return new CommandInfo(this);
        }

        public HashSet<string> GetRequiredArgs()
        {
            var requiredArgs = new HashSet<string>();
            if (Arguments is not null)
                foreach (var key in Arguments.Keys) if (Arguments[key] is RequiredArgument) requiredArgs.Add(key);
            return requiredArgs; 
        }
        public void SetArgumentValue(string argumentName, string argumentValue)
        {
            if (Arguments is null || !Arguments.TryGetValue(argumentName, out BaseArgument? argument)) throw new CliCommandArgumentNotFoundException(argumentName);
            argument.Value(argumentValue);
        }

        public int ExpectedArgumentsCount => (Arguments is not null) ? Arguments.Keys.Count : 0;
    }
    public abstract class BaseNamespaceCommand: BaseCommand
    {
        public override CommandType Type => CommandType.NameSpace;

        public override bool Verify(ref AppStateProxy Context)
        {
            if (Context.PeekNextArg is null) throw new CliCommandInvalidException($"Invalid use of command: {Name}");
            return !(Context.PeekNextArg == "help");
        }
    }

    public abstract class BaseActionCommand: BaseCommand
    {
        public override CommandType Type => CommandType.Action;
        public override bool Verify(ref AppStateProxy appProxy)
        {
            int commandExpectedArgCount = (Arguments is not null) ? Arguments.Keys.Count : 0;
            // Handles when we expecting arguments
            if (commandExpectedArgCount > 0) 
            {
                // We recieved no arguments
                if (!appProxy.HasNextArg) throw new CliCommandInvalidException($"Missing arguments for command {Name}");
                else {
                    appProxy.SetCommandArgs(this);
                }
            }
            // Handles when we not expecting an argument but have some
            else if (commandExpectedArgCount == 0 && appProxy.HasNextArg)
            {
                if (appProxy.PeekNextArg != "help") throw new CliCommandInvalidException($"{appProxy.PreviousRanCommand?.CommandName} Doesn't take any arguments");
                return false;
            }
            return true;
        }
    }

    public abstract class BaseArgument
    {
        private string? _value = null;
        public string name = string.Empty;
        public string Value()
        {
            if (_value is null) throw new CliCommandRequiredArgumentException($"Argument: {name} is required");
            return _value;
        }
        public void Value(string value)
        {
            _value ??= value;
        }
    }

    public class OptionalArgument: BaseArgument
    {
        public OptionalArgument(string defaultVaule) : base()
        {
            Value(defaultVaule);
        }
    }

    public class RequiredArgument: BaseArgument
    {
    }
}
