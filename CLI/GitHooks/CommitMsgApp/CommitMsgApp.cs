using System.Text.RegularExpressions;
using Shared;

namespace Cli.GitHooks
{
    public partial class CommitMsgApp 
    {
        private static readonly Regex regex = FlagRegex();
        public static void Main(string[] args)
        {
            string commitMsgFile = args[0];
            UpdateFlaggedCommitFile(commitMsgFile);
        }

        // Builds regex at compile time
        [GeneratedRegex(@"^ctts-explain:")]
        private static partial Regex FlagRegex();

        public static void UpdateFlaggedCommitFile(string commitMsgFile)
        {
            CliFileHelper fileHelper = new(commitMsgFile);
            string contents = fileHelper.ReadFile();
            if (regex.Matches(contents).Count > 0) fileHelper.UpdateFileContents(contents.Remove(0, "ctts-explain:".Length));
        }
    }
}