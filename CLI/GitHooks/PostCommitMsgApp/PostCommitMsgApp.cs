using Shared;
using OpenaiSummarizer;
using CliApp.CommandLine.Context;
using Cli.GitHooks.Services.AuthService;

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
                var accessToken = "";
                var authService = new AuthService();
                if(authService.GetAccessToken() == "")
                {
                    accessToken = await authService.GetAccessTokenAsync(Environment.GetEnvironmentVariable("GitCLI_AuthURL"))!;

                }
                else
                {
                    accessToken = authService.GetAccessToken();
                }
                authService.GetAccessToken();
                Console.WriteLine($"Access Token: {accessToken}");

                Program.Main([new GitWrapper().GetDiffForPreviousCommit().Replace('"', '\'')]);
                // fileHelper.DeletePath("CTTS_COMMIT_FLAG");
            }
        }
    }
}
