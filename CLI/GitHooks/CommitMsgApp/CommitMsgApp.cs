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
            var fileHelper = UpdateFlaggedCommitFile(commitMsgFile);
            if (fileHelper is not null) CreateFlagFile(Path.Join(fileHelper.GetDirectoryPath(), "hooks"));
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
            // parentDirectory = Path.Join([parentDirectory, "hooks"]);
            // Should error out if hooks directory doesn't exist
            fileHelper = new(parentDirectory);
            return fileHelper.CreateInPath("CTTS_FLAGGED_COMMIT");
        }
    }
}