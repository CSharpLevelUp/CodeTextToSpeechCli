using Cli.GitHookse.Services;
using CliApp.CommandLine.DataClasses;
using System.Diagnostics;
using System.Text.Json;

namespace Cli.GitHooks.Services.AuthService
{
    public class AuthService
    {
        private readonly HttpServer _httpServer;
        private String _redirectUri;
        private String _accessToken;

        public AuthService()
        {
            _redirectUri = Environment.GetEnvironmentVariable("GitCLI_RedirectURI")!;
            _httpServer = new HttpServer(_redirectUri);
        }

        public void browserAuth()
        {
            string auth0Domain = "dev-01dgwqpxxjssj0wd.us.auth0.com";
            string clientId = Environment.GetEnvironmentVariable("gitCLI_ClientID")!;
            string redirectUri = Environment.GetEnvironmentVariable("GitCLI_RedirectURI")!.ToString();

            UriBuilder uriBuilder = new UriBuilder("https", auth0Domain);
            uriBuilder.Path = "/authorize";

            // Add query parameters
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["client_id"] = clientId;
            query["redirect_uri"] = redirectUri;
            query["response_type"] = "code";
            uriBuilder.Query = query.ToString();

            // Get the full authorization URL
            string authUrl = uriBuilder.ToString();
            Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });
        }

        public async Task<string> GetAccessTokenAsync(string tokenEndpoint)
        {
            browserAuth();
            await _httpServer.StartAsync();
            string authorizationCode = await _httpServer.WaitForAuthorizationCodeAsync();

            var tokenRequest = new TokenRequest
            {
                ClientId = Environment.GetEnvironmentVariable("GitCLI_ClientID")!,
                ClientSecret = Environment.GetEnvironmentVariable("GitCLI_ClientSecret")!,
                Code = authorizationCode,
                RedirectUri = _redirectUri,
                GrantType = "authorization_code"
            };
            
            using var httpClient = new HttpClient();
            var tokenRequestContent = new FormUrlEncodedContent(tokenRequest.ToDictionary());

            var response = await httpClient.PostAsync(tokenEndpoint, tokenRequestContent);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                if (tokenResponse.error != null)
                {
                    throw new Exception($"Failed to retrieve access token. Error: {tokenResponse.error}");
                }
                _accessToken = tokenResponse.access_token;
                var id = tokenResponse.id_token;
                return _accessToken;
            }
            else
            {
                throw new Exception($"Failed to retrieve access token. Status code: {response.StatusCode}");
            }
        }

        public string GetAccessToken()
        {
            return _accessToken;
        }

        
    }
}