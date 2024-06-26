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
        private TokenResponse _tokenResponse;

        public AuthService()
        {
            _redirectUri = "http://localhost:5000/";
            _httpServer = new HttpServer(_redirectUri);
        }

        public void browserAuth()
        {
            string auth0Domain = "dev-01dgwqpxxjssj0wd.us.auth0.com";
            string clientId = "ciYKIQ6wWxIdmGMzD4CAeofDxOM5LGTE";
            string redirectUri = "http://localhost:5000/";

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

        public async Task<TokenResponse> GetAccessTokenAsync(string tokenEndpoint)
        {
            browserAuth();
            await _httpServer.StartAsync();
            string authorizationCode = await _httpServer.WaitForAuthorizationCodeAsync();

            var tokenRequest = new TokenRequest
            {
                ClientId = "ciYKIQ6wWxIdmGMzD4CAeofDxOM5LGTE",
                ClientSecret = Environment.GetEnvironmentVariable("GitCLI_ClientSecret")!, // {{AUTH_CLIENT_ID}}
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
                _tokenResponse = tokenResponse;
                return tokenResponse;
            }
            else
            {
                throw new Exception($"Failed to retrieve access token. Status code: {response.StatusCode}");
            }
        }

        public TokenResponse GetAccessToken()
        {
            return _tokenResponse;
        }

        
    }
}