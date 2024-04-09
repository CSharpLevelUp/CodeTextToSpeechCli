﻿using CliApp.CommandLine.CommandBase;

namespace CliApp.CommandLine.DataClasses
{
    public class CommandInfo(BaseCommand command)
    {
        public readonly CommandType Type = command.Type;
        public readonly string HelpText = command.HelpText;

        public readonly string CommandName = command.Name;

        public override bool Equals(object? obj)
        {
            if (obj is not CommandInfo commandInfo) return false;
            return Type == command.Type && CommandName == commandInfo.CommandName;
        }
    }

    public enum CommandType
    {
        NameSpace = 0,
        Action = 1
    }
}