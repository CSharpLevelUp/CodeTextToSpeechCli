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
            var flagPath = Path.Join([fileHelper.CurrentPath,"CTTS_FLAGGED_COMMIT"]);
            if (File.Exists(flagPath))
            // if (true)
            {
                TokenResponse accessAuth = null;
                var authService = new AuthService();
                if (authService.GetAccessToken() == null)
                {
                    accessAuth = await authService.GetAccessTokenAsync("https://dev-01dgwqpxxjssj0wd.us.auth0.com/oauth/token");
                }
                else
                {
                    accessAuth = authService.GetAccessToken();
                }

                try 
                {
                    string path = Path.Join([Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "CodeTextToSpeech", "openai-key"]);
                    CliFileHelper fileHelper = new(path);
                    string diff = new GitWrapper().GetDiffForPreviousCommit().Replace('"', '\'');
                    Console.WriteLine($"diff:\n {diff}");
                    string summary = OpenAIHelper.GetDiffSummary(diff, fileHelper.ReadFile());
                    Console.WriteLine($"summary:\n {summary}");
                    OpenCommandPrompt(summary);
                    if (summary != null) BackendClient.SendCommitSummary(diff, summary, accessAuth.access_token);
                    else throw new Exception("Error: Unable to fetch summary from OpenAI API.");
                } catch(CliFileHelperException e)
                {
                    CliFileHelper.CreateAppDirInLocalAppDataIfNotExist();
                    throw new CliCommandInvalidException("Register your open ai api key by running the set-openai-api-key --key=<openai api key>");
                }
                new CliFileHelper(flagPath).DeletePath();
            }
        }

        public static void OpenCommandPrompt(string summary)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"echo \"{summary}\"";
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
