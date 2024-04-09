using Shared;

namespace Tests;
[TestClass]
public class CliFileHelperTests
{
    private readonly string workingDir = "../../../Resources/";
    private readonly string newContent = "Hello from new content";
    
    [TestMethod]
    public void TestUpdateFileContentsFromFile()
    {
        CliFileHelper fileHelper = new(Path.Join(workingDir, "update_content_from_file.txt"));
        fileHelper.UpdateFileContents(newContent);
        Assert.AreEqual(newContent, fileHelper.ReadFile());
        fileHelper.UpdateFileContents("");
        Assert.AreEqual("", fileHelper.ReadFile());
    }

    [TestMethod]
    public void TestUpdateFileContentsFromDir()
    {
        string filename = "update_content_from_dir.txt";
        CliFileHelper fileHelper = new(workingDir);
        fileHelper.UpdateFileContents(newContent, filename);
        Assert.AreEqual(newContent, fileHelper.ReadFile(filename));
        fileHelper.UpdateFileContents("", filename);
        Assert.AreEqual("", fileHelper.ReadFile(filename));
    }

    [TestMethod]
    public void TestGetDirectoryName()
    {
        CliFileHelper textFileHelper = new(Path.Join(workingDir, "update_content_from_file.txt"));
        CliFileHelper dirFileHelper = new(workingDir);
        Assert.AreEqual(dirFileHelper.CurrentPath[0..^1], textFileHelper.GetDirectoryPath());
    }
}
