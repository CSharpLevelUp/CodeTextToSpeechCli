using System.Diagnostics;
using System.Text.Encodings.Web;
using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Context;

namespace CliApp.CommandLine.Commands
{
    public class AuthCommand: BaseActionCommand
    {
        public override string Name => "auth";
        public override string HelpText => "Auth Command Help";

        public override Dictionary<string, BaseArgument>? Arguments => null;

        public override void Execute(ref AppStateProxy appProxy)
        {
            Console.WriteLine("Executing Auth Command");
            string auth0Domain = "dev-01dgwqpxxjssj0wd.us.auth0.com"; 
            string clientId = Environment.GetEnvironmentVariable("gitCLI_ClientID")!;
            string redirectUri = Environment.GetEnvironmentVariable("GitCLI_RedirectURI")!.ToString();

            UriBuilder uriBuilder = new UriBuilder("https", auth0Domain);
            uriBuilder.Path = "/authorize";

            // Add query parameters
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["client_id"] = clientId;
            query["redirect_uri"] = redirectUri;
            query["response_type"] = "code";
            uriBuilder.Query = query.ToString();

            // Get the full authorization URL
            string authUrl = uriBuilder.ToString();
            Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });
        }
    }
}
