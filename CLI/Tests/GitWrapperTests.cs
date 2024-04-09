using System.Text;
using Shared;

namespace Tests;

[TestClass]
public class GitWrapperTests
{
    private readonly string workingDir = "../../../Resources/";

    [TestMethod]
    public void TestGetDiffForPreviousCommit()
    {
        string diff = new GitWrapper($"{workingDir}/web-basics-hello-world").GetDiffForPreviousCommit();
        Assert.AreEqual(new CliFileHelper(workingDir).ReadFile("two_consecutive_commits.diff"), diff);
    }

    [TestMethod]
    public void TestGetFileRevisionCommitHashesRange1()
    {
        GitWrapper git = new($"{workingDir}/web-basics-hello-world");
        var hashes = git?.GetFileRevisionCommitHashes("index.css");
        var expectedHash = "a716015cbb15292f808967499de99a356a769378";
        Assert.IsNotNull(hashes);
        Assert.IsTrue(hashes.Length != 0);
        Assert.AreEqual(expectedHash, hashes?[0]);
    }

    [TestMethod]
    public void TestGetFileRevisionCommitHashesRange2()
    {
        GitWrapper git = new($"{workingDir}/web-basics-hello-world");
        var hashes = git.GetFileRevisionCommitHashes("index.css", 2);
        string[] expectedHashes = [
            "6490aac5df2d62d71ed8e05eba04e0a6c380f515",
            "a716015cbb15292f808967499de99a356a769378"
        ];
        Assert.IsNotNull(hashes);
        Assert.IsTrue(hashes.Length != 0);
        for (int idx = 0; idx < hashes.Length; idx++) Assert.AreEqual(expectedHashes[idx], hashes[idx]);
    }

    [TestMethod]
    public void TestGetCommitHashesAndFilesAtHEAD()
    {
        var git = new GitWrapper($"{workingDir}/web-basics-hello-world");
        var commitFiles = git.GetCommitHashesAndFiles();
        Assert.AreEqual("62716c9f0e3d93a536f1741c2a4af9597084c82a", commitFiles.Hash);
        string[] expectedFiles = ["README.md"];
        Assert.AreEqual(expectedFiles.Length, commitFiles.Files.Length);
        for (int idx=0; idx < commitFiles.Files.Length; idx++) Assert.AreEqual(expectedFiles[idx], commitFiles.Files[idx]);
    }

    
    [TestMethod]
    public void TestGetCommitHashesAndFilesAt1StepFromHEAD()
    {
        var git = new GitWrapper($"{workingDir}/web-basics-hello-world");
        var commitFiles = git.GetCommitHashesAndFiles(1);
        Assert.AreEqual("a716015cbb15292f808967499de99a356a769378", commitFiles.Hash);
        string[] expectedFiles = ["README.md", "index.css", "index.html", "index.js"];
        Assert.AreEqual(expectedFiles.Length, commitFiles.Files.Length);
        for (int idx=0; idx < commitFiles.Files.Length; idx++) Assert.AreEqual(expectedFiles[idx], commitFiles.Files[idx]);
    }

    [TestMethod]
    public void TestGitSubmodule()
    {
        string root = "../../../../../";
        var git = new GitWrapper($"{workingDir}/web-basics-hello-world");
        Assert.IsTrue(git.IsSubmodule);
        Assert.IsNotNull(git.Submodule);
        Assert.AreEqual(Path.GetFullPath(root)[0..^1], Path.GetFullPath(git.Submodule.ParentRepoDirectory));
        Assert.AreEqual(Path.GetFullPath(Path.Join([root, ".git", "modules", "CLI", "Tests", "Resources", "web-basics-hello-world"])), git.Submodule.GitDirectory);
    }
}