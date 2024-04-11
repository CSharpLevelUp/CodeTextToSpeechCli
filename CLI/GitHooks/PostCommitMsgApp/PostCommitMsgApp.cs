using Shared;
using CliApp.CommandLine.Exceptions;
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
            // if (flagSearch is not null)
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

                try 
                {
                    string path = Path.Join([Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "CodeTextToSpeech", "openai-key"]);
                    CliFileHelper fileHelper = new(path);
                    string diff = new GitWrapper().GetDiffForPreviousCommit().Replace('"', '\'');
                    string summary = OpenAIHelper.GetDiffSummary(diff, fileHelper.ReadFile());
                    if (summary != null) BackendClient.SendCommitSummary(diff, summary);
                    else throw new Exception("Error: Unable to fetch summary from OpenAI API.");
                } catch(CliFileHelperException e)
                {
                    CliFileHelper.CreateAppDirInLocalAppDataIfNotExist();
                    throw new CliCommandInvalidException("Register your open ai api key by running the set-openai-api-key --key=<openai api key>");
                }
                // fileHelper.DeletePath("CTTS_COMMIT_FLAG");
            }
        }
    }
}
