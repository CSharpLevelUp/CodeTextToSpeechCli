using CliApp.CommandLine.CommandBase;
using CliApp.CommandLine.Context;
using Shared;

namespace CliApp.CommandLine.Commands
{
    public class SetOpenAiApiKeyCommand: BaseActionCommand
    {
        public override string Name => "set-openai-api-key";
        public override string HelpText => "Sets the open ai api key";

        private Dictionary<string, BaseArgument> _arguments =  new()
        {
            { "key", new RequiredArgument("The open ai key") },
        };
        public override Dictionary<string, BaseArgument>? Arguments => _arguments;

        public override void Execute(ref AppStateProxy appProxy)
        {
            string appDataDirPath = CliFileHelper.CreateAppDirInLocalAppDataIfNotExist();
            CliFileHelper cliFileHelper = new(appDataDirPath);
            if (Arguments is null) throw new ArgumentNullException($"Command {this.Name} arguments are null when value is expected");
            if (Arguments["key"].Value is null) throw new ArgumentNullException($"Command argument key is null");
            cliFileHelper = (!File.Exists(Path.Join(appDataDirPath, "openai-key"))) ? new(cliFileHelper.CreateInPath("openai-key")) : new(Path.Join([appDataDirPath, "openai-key"]));
            cliFileHelper.UpdateFileContents(Arguments["key"].Value);
        }
    }
}
