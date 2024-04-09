using Cli.GitHooks;
using Shared;

namespace Tests;

[TestClass]
public class CommitMsgAppTests
{
    private readonly string workingDir = "../../../Resources/";
    [TestMethod]
    public void TestUpdateFlaggedCommitFile()
    {
        string filePath = Path.Join([workingDir, "COMMIT_EDITMSG"]);
        CliFileHelper fileHelper = new(filePath);
        string oldCommitMsg = fileHelper.ReadFile();
        CommitMsgApp.UpdateFlaggedCommitFile(filePath);
        Assert.AreEqual(fileHelper.ReadFile(), "Added logic to remove flags in commit message");
        fileHelper.UpdateFileContents(oldCommitMsg);
    }
}
