using CliApp;
namespace Tests;

[TestClass]
public class CliAppTests
{
    [TestMethod]
    public void TestCliAppRanWithNoCommand()
    {
        CliApp.CliApp.Main([]);
    }
}
