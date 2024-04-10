using System.Net;
using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Context;
using Shared;

namespace CliApp.CommandLine.Commands
{
    public class HookLocalRepoCommand: BaseActionCommand
    {
        public override string Name => "hook-local-repo";
        public override string HelpText => "Registers Git Hooks on local repo";
        public override Dictionary<string, BaseArgument>? Arguments => new Dictionary<string, BaseArgument>
        {
            { "folder-path", new RequiredArgument("Local path to the repo directory") }
        };

        public override void Execute(ref AppStateProxy appProxy)
        {
            using(WebClient client = new())
            {
                string[] fileUrls = [
                    "https://github.com/CSharpLevelUp/CodeTextToSpeechCli/releases/download/v0.0.1-feature-cli-ci-cd.7/commit-msg.exe",
                    "https://github.com/CSharpLevelUp/CodeTextToSpeechCli/releases/download/v0.0.1-feature-cli-ci-cd.7/post-commit.exe"
                ];

                foreach(string fileUrl in fileUrls)
                {
                    var filename = Path.GetFileName(fileUrl);
                    client.DownloadFile(fileUrl, filename);
                    if (Arguments is null) throw new ArgumentNullException($"Command {this.Name} arguments are null when value is expected");
                    GitWrapper gitWrapper = new(Arguments["folder-path"].Value());
                    var outputPath = Path.Combine(
                        (gitWrapper.IsSubmodule && gitWrapper.Submodule is not null) 
                            ? gitWrapper.Submodule.GitDirectory 
                            : gitWrapper.WorkingDirectory, 
                        "hooks");
                    File.Move(Path.GetFullPath(Path.Combine("./", filename)), outputPath);
                }
            }
        }
    }
}
