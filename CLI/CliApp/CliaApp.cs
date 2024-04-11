using CliApp.CommandLine.Context;
using Shared;

namespace CliApp
{
    public class CliApp
    {
        public static async Task Main(string[] args)
        {
            new CommandInvoker(new AppState(args)).Run();
        }
    }
}