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
        public Dictionary<string, BaseArgument> _arguments = new()
        {
            { "folder-path", new RequiredArgument("Local path to the repo directory") }
        };
        public override Dictionary<string, BaseArgument>? Arguments => _arguments;

        public override void Execute(ref AppStateProxy appProxy)
        {
            using(WebClient client = new())
            {
                string[] fileUrls = [
                    "https://github.com/CSharpLevelUp/CodeTextToSpeechCli/releases/download/v0.0.2-feature-cli-ci-cd.6/commit-msg.exe",
                    "https://github.com/CSharpLevelUp/CodeTextToSpeechCli/releases/download/v0.0.2-feature-cli-ci-cd.6/post-commit.exe"
                ];

                foreach(string fileUrl in fileUrls)
                {
                    var filename = Path.GetFileName(fileUrl);
                    client.DownloadFile(fileUrl, filename);
                    if (Arguments is null) throw new ArgumentNullException($"Command {this.Name} arguments are null when value is expected");
                    if (Arguments["folder-path"].Value is null) throw new ArgumentNullException($"Command argument folder-path is null");
                    GitWrapper gitWrapper = new(Arguments["folder-path"].Value);
                    var outputPath = Path.Join([
                        (gitWrapper.IsSubmodule && gitWrapper.Submodule is not null) 
                            ? gitWrapper.Submodule.GitDirectory 
                            : gitWrapper.WorkingDirectory, 
                        ".git", "hooks"]);
                    File.Move(Path.GetFullPath(Path.Combine("./", filename)), Path.Join([outputPath, filename]));
                }
            }
        }
    }
}
