﻿using CliApp.CommandLine.Context;

namespace CliApp
{
    public class CliApp
    {
        public static void Main(string[] args)
        {
            new CommandInvoker(new CliAppContext(args)).Run();
        }
    }
}