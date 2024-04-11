using System.Diagnostics;
using System.Text.Encodings.Web;
using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Context;

namespace CliApp.CommandLine.Commands
{
    public class AuthCommand: BaseActionCommand
    {
        public override string Name => "auth";
        public override string HelpText => "Start a SSO session";

        public override Dictionary<string, BaseArgument>? Arguments => null;

        public override void Execute(ref AppStateProxy appProxy)
        {
            Console.WriteLine("Executing Auth Command");
            
        }
    }
}
