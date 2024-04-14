using System.Text.Json;
using System.Text.RegularExpressions;
using Cli.GitHooks.Services.AuthService;
using CliApp.CommandLine.DataClasses;
using CliApp.CommandLine.Exceptions;
using OpenaiSummarizer;
using Shared;
using static OpenaiSummarizer.BackendClient;

namespace Cli.GitHooks
{
    public partial class CommitMsgApp 
    {
        private static readonly Regex regex = FlagRegex();
        public static async Task Main(string[] args)
        {
            string commitMsgFile = args[0];
            var fileHelper = UpdateFlaggedCommitFile(commitMsgFile);
            if (fileHelper is not null) await AuthUser();
        }

        // Builds regex at compile time
        [GeneratedRegex(@"^ctts-explain:")]
        private static partial Regex FlagRegex();

        public static CliFileHelper? UpdateFlaggedCommitFile(string commitMsgFile)
        {
            CliFileHelper fileHelper = new(commitMsgFile);
            string contents = fileHelper.ReadFile();
            if (regex.Matches(contents).Count > 0) 
            {
                fileHelper.UpdateFileContents(contents.Remove(0, "ctts-explain:".Length));
                return fileHelper;
            }
            return null;
        }

        public static string CreateFlagFile(string path)
        {
            CliFileHelper fileHelper = new(path);
            string parentDirectory = (fileHelper.IsDirectory) ? fileHelper.CurrentPath: fileHelper.GetDirectoryPath();
            parentDirectory = Path.Join([parentDirectory, "hooks"]);
            // Should error out if hooks directory doesn't exist
            fileHelper = new(parentDirectory);
            return fileHelper.CreateInPath("CTTS_FLAGGED_COMMIT");
        }

        public static async Task SaveAccesssToken()
        {
            string diff = new GitWrapper().GetDiffForPreviousCommit().Replace('"', '\'');
            CliFileHelper fileHelper = new(".");
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