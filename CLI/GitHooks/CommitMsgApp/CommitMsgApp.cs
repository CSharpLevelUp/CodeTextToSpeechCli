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
            CliFileHelper fileHelper = new(commitMsgFile);
            string contents = fileHelper.ReadFile();
            if (regex.Matches(contents).Count > 0) fileHelper.UpdateFileContents(contents.Remove(0, "ctts-explain:".Length));
        }

        [GeneratedRegex(@"^ctts-explain:")]
        private static partial Regex FlagRegex();
    }
}