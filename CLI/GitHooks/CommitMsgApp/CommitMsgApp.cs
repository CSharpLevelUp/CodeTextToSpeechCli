using Shared;

namespace Cli.GitHooks
{
    public class CommitMsgApp 
    {
        public static void Main(string[] args)
        {
            string commitMsgFile = args[0];
            CliFileHelper fileHelper = new(commitMsgFile);

            
        }

        private static void UpdateFileContent(string commitMsgFile, string content)
        {
            
        }
    }
}