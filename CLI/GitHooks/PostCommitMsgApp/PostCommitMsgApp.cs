using Shared;
using CliApp.CommandLine.Exceptions;
using OpenaiSummarizer;
using CliApp.CommandLine.Context;
using Cli.GitHooks.Services.AuthService;
using CliApp.CommandLine.DataClasses;
using System.Text.Json;
using static OpenaiSummarizer.BackendClient;

namespace Cli.GitHooks
{
    public class PostCommitMsgApp
    {
        private static readonly CliFileHelper fileHelper = new(".");
        public static async Task Main(string[] args)
        {
            // var flagPath = Path.Join([new ProcessRunner("git", ".").RunCommand("rev-parse --absolute-git-dir"), "hooks", "CTTS_FLAGGED_COMMIT"]);
            ///if (File.Exists(flagPath))
            if (true)
            {
                string diff = new GitWrapper().GetDiffForPreviousCommit().Replace('"', '\'');
                bool isNewAuth = false;
                try 
                {
                    TokenResponse accessAuth;
                    if (fileHelper.SearchInLowestDirectory("user-auth-token.json") is not null)
                    {                    
                        accessAuth = JsonSerializer.Deserialize<TokenResponse>(fileHelper.ReadFile());
                    } else {
                        accessAuth = await AuthUser();
                        SaveTokenFile(accessAuth);
                        isNewAuth = true;
                    }
                    string summary = GetSummary(diff);
                    if (summary != null) BackendClient.SendCommitSummary(diff, summary, accessAuth.access_token);
                    else throw new Exception("Error: Unable to fetch summary from OpenAI API.");
                } catch(CliFileHelperException)
                {
                    CliFileHelper.CreateAppDirInLocalAppDataIfNotExist();
                    throw new CliCommandInvalidException("Register your open ai api key by running the set-openai-api-key --key=<openai api key>");
                } catch(BackendClientException)
                {
                    if (isNewAuth) throw new Exception("Can't connect to Backend");
                    var accessAuth = await AuthUser();
                    SaveTokenFile(accessAuth);
                    string summary = GetSummary(diff);
                    if (summary != null) BackendClient.SendCommitSummary(diff, summary, accessAuth.access_token);
                    else throw new Exception("Error: Unable to fetch summary from OpenAI API.");
                }
                //new CliFileHelper(flagPath).DeletePath();
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

        public static async Task<TokenResponse> AuthUser()
        {
            var authService = new AuthService();
            TokenResponse accessAuth = (authService.GetAccessToken() == null) ?
                await authService.GetAccessTokenAsync("https://dev-01dgwqpxxjssj0wd.us.auth0.com/oauth/token"):
                authService.GetAccessToken();

            Console.WriteLine($"Access Token: {accessAuth.access_token}");
            Console.WriteLine($"ID Token: {accessAuth.ToString()}");
            return accessAuth;
        }

        public static string GetSummary(string diff)
        {
            string path = Path.Join([Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "CodeTextToSpeech", "openai-key"]);
            CliFileHelper fileHelper = new(path);
            string summary = OpenAIHelper.GetDiffSummary(diff, fileHelper.ReadFile());
            OpenCommandPrompt(summary);
            return summary;
        }

        public static void SaveTokenFile(TokenResponse accessAuth)
        {
            string tokenPath = Path.Join([Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "CodeTextToSpeech"]);
            CliFileHelper fileHelper = new(tokenPath);
            var tokenFilePath = Path.Join([tokenPath, "user-auth-token.json"]);
            if (!File.Exists(tokenFilePath))
                CliFileHelper.CreateInLocalAppData(Path.Join(["CodeTextToSpeech", "user-auth-token.json"]));
            new CliFileHelper(tokenFilePath).UpdateFileContents(JsonSerializer.Serialize<TokenResponse>(accessAuth));
        }
    }
}
