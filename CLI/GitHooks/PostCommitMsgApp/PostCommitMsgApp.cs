using Shared;

namespace Cli.GitHooks
{
    public class PostCommitMsgApp
    {
        private static readonly GitWrapper gitWrapper = new(); 
        private static readonly CliFileHelper fileHelper = new(".");
        public static void Main(string[] args)
        {
            var flagSearch = fileHelper.SearchInLowestDirectory("CTTS_COMMIT_FLAG");
            if (flagSearch is not null)
            {
                GitCommitFiles previousHashFiles = gitWrapper.GetCommitHashAndFiles();
                foreach(string file in previousHashFiles.Files)
                {
                    string diff = gitWrapper.GetDiffForPreviousCommit(file);
                    // Send diffs to open-ai
                }
                fileHelper.DeletePath("CTTS_COMMIT_FLAG");
            }
        }
    }
}
