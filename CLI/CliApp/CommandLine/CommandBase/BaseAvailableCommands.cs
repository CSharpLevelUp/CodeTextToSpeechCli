using System.Collections.Immutable;
using CliApp.CommandLine.Config;
using CliApp.CommandLine.DataClasses;
using CliApp.CommandLine.Exceptions;

namespace CliApp.CommandLine.CommandBase
{
    public abstract class BaseAvailableCommands
    {
        public abstract BaseCommand[] AvailableCommands { get;}
        public readonly ImmutableDictionary<string, BaseCommand> CommandsMap;
        public BaseAvailableCommands()
        {
            if (AvailableCommands is null) throw new CliCommandException("AvailableCommands List can't be null, instead return an empty array");
            var mergedCommands = MergeCommands();
            var KeyValuePairArray = new KeyValuePair<string, BaseCommand>[mergedCommands.Length];

            for (int idx = 0; idx < mergedCommands.Length; idx++)
            {
                KeyValuePairArray[idx] = KeyValuePair.Create(mergedCommands[idx].Name, mergedCommands[idx]);
            }
            CommandsMap = ImmutableDictionary.CreateRange(KeyValuePairArray);
        }

        private BaseCommand[] MergeCommands()
        {
            return[.. AvailableCommands, .. GlobalCommands.Commands];
        }
    }
}
