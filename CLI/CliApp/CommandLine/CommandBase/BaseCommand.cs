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

        public abstract CommandVerifyOutcome Verify(ref AppStateProxy appProxy);
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
            Arguments[argumentName].Value = argumentValue;
        }

        public int ExpectedArgumentsCount => (Arguments is not null) ? Arguments.Keys.Count : 0;
    }
    public abstract class BaseNamespaceCommand: BaseCommand
    {
        public override CommandType Type => CommandType.NameSpace;

        public override CommandVerifyOutcome Verify(ref AppStateProxy Context)
        {
            if (Context.PeekNextArg is null) throw new CliCommandInvalidException($"Invalid use of command: {Name}");
            return !(Context.PeekNextArg == "help") ? CommandVerifyOutcome.Success : CommandVerifyOutcome.Failure;
        }
    }

    public abstract class BaseActionCommand: BaseCommand
    {
        public override CommandType Type => CommandType.Action;
        public override CommandVerifyOutcome Verify(ref AppStateProxy appProxy)
        {
            int commandExpectedArgCount = (Arguments is not null) ? Arguments.Keys.Count : 0;
            // Handles when we expecting arguments
            if (commandExpectedArgCount > 0) 
            {
                // We recieved no arguments
                if (!appProxy.HasNextArg) throw new CliCommandInvalidException($"Missing arguments for command {Name}");
                else {
                    return CommandVerifyOutcome.SetArguments;
                }
            }
            // Handles when we not expecting an argument but have some
            else if (commandExpectedArgCount == 0 && appProxy.HasNextArg)
            {
                if (appProxy.PeekNextArg != "help") throw new CliCommandInvalidException($"{appProxy.PreviousRanCommand?.CommandName} Doesn't take any arguments");
                return CommandVerifyOutcome.Failure;
            }
            return CommandVerifyOutcome.Success;
        }
    }

    public class BaseArgument
    {
        private string? _description = null;
        public string Value { get; set; }
        public string? Description
        {
            get { return _description; }
            set { if(value is not null) _description ??= value; }
        }
    }

    public class OptionalArgument: BaseArgument
    {
        public OptionalArgument(string description, string defaultValue="") : base()
        {
            Value = defaultValue;
            Description = description;
        }
    }

    public class RequiredArgument: BaseArgument
    {
        public RequiredArgument(string description) : base()
        {
            Description = description;
        }
    }
}
