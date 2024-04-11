using Shared;
using OpenaiSummarizer;

namespace Cli.GitHooks
{
    public class PostCommitMsgApp
    {
        private static readonly CliFileHelper fileHelper = new(".");
        public static void Main(string[] args)
        {
            var flagSearch = fileHelper.SearchInLowestDirectory("CTTS_COMMIT_FLAG");
            if (true)
            {
                Program.Main([new GitWrapper().GetDiffForPreviousCommit().Replace('"', '\'')]);
                // fileHelper.DeletePath("CTTS_COMMIT_FLAG");
            }
        }
    }
}
