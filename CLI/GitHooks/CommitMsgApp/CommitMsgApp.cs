using System.Text.RegularExpressions;
using Cli.GitHooks.Services.AuthService;
using CliApp.CommandLine.DataClasses;
using CliApp.CommandLine.Exceptions;
using OpenaiSummarizer;
using Shared;

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

        public static async Task AuthUser()
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
                if (summary != null) BackendClient.SendCommitSummary(diff, summary, accessAuth.access_token);
                else throw new Exception("Error: Unable to fetch summary from OpenAI API.");
            } catch(CliFileHelperException e)
            {
                CliFileHelper.CreateAppDirInLocalAppDataIfNotExist();
                throw new CliCommandInvalidException("Register your open ai api key by running the set-openai-api-key --key=<openai api key>");
            }
        }
    }
}