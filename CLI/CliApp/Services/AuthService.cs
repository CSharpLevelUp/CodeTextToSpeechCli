using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CliApp.CommandLine.DataClasses;
using CliApp.CommandLine.Services;

namespace CliApp.CommandLine.Services.AuthService
{
    public class AuthService
    {
        private readonly HttpServer _httpServer;
        private String _redirectUri;

        public AuthService()
        {
            _redirectUri = Environment.GetEnvironmentVariable("GitCLI_RedirectURI")!;
            Console.WriteLine($"Redirect URI: {_redirectUri}");
            _httpServer = new HttpServer(_redirectUri);
        }

        public async Task<string> GetAccessTokenAsync(string tokenEndpoint)
        {
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
                return tokenResponse.access_token;
            }
            else
            {
                throw new Exception($"Failed to retrieve access token. Status code: {response.StatusCode}");
            }
        }
    }
}