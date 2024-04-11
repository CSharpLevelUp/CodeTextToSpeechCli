using CliApp.CommandLine.CommandBase;

namespace CliApp.CommandLine.DataClasses
{
    public class CommandInfo(BaseCommand command)
    {
        public readonly CommandType Type = command.Type;
        public readonly string HelpText = command.HelpText;

        public readonly string CommandName = command.Name;

        public override bool Equals(object? obj)
        {
            if (obj is not CommandInfo commandInfo) return false;
            return Type == command.Type && CommandName == commandInfo.CommandName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum CommandType
    {
        NameSpace = 0,
        Action = 1
    }

    class TokenResponse
{
    public string access_token { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public string error { get; set; }
    // Add additional properties as needed
}

public class TokenRequest
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Code { get; set; }
    public string RedirectUri { get; set; }
    public string GrantType { get; set; }

    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
    {
        { "client_id", ClientId },
        { "client_secret", ClientSecret },
        { "code", Code },
        { "redirect_uri", RedirectUri },
        { "grant_type", GrantType }
    };
    }
}
    public enum CommandVerifyOutcome
    {
        Success = 0,
        Failure = 1, 
        SetArguments = 2
    }
}