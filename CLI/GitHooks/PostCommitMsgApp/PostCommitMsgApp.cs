using Shared;
using OpenaiSummarizer;
using CliApp.CommandLine.Context;
using Cli.GitHooks.Services.AuthService;
using CliApp.CommandLine.DataClasses;

namespace Cli.GitHooks
{
    public class PostCommitMsgApp
    {
        private static readonly CliFileHelper fileHelper = new(".");
        public static async Task Main(string[] args)
        {
            var flagSearch = fileHelper.SearchInLowestDirectory("CTTS_COMMIT_FLAG");
            if (true)
            {
                TokenResponse accessAuth = null;
                var authService = new AuthService();
                if (authService.GetAccessToken() == null)
                {
                    accessAuth = await authService.GetAccessTokenAsync(Environment.GetEnvironmentVariable("GitCLI_AuthURL"));
                }
                else
                {
                    accessAuth = authService.GetAccessToken();
                }
                Console.WriteLine($"Access Token: {accessAuth.access_token}");
                Console.WriteLine($"ID Token: {accessAuth.ToString()}");

                Program.Main([new GitWrapper().GetDiffForPreviousCommit().Replace('"', '\''), accessAuth.access_token]);
                // fileHelper.DeletePath("CTTS_COMMIT_FLAG");
            }
        }
    }
}
