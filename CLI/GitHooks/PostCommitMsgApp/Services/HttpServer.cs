using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cli.GitHookse.Services
{
    public class HttpServer
    {
        private readonly HttpListener _httpListener;
        private readonly string _redirectUri;
        private readonly TaskCompletionSource<string> _authorizationCodeCompletionSource;

        public HttpServer(string redirectUri)
        {
            _redirectUri = redirectUri;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(redirectUri);
            _authorizationCodeCompletionSource = new TaskCompletionSource<string>();
        }

        public async Task StartAsync()
        {
            try
            {
                _httpListener.Start();
                Console.WriteLine($"HTTP server started. Listening for requests on {_redirectUri}");

                while (_httpListener.IsListening)
                {
                    var context = await _httpListener.GetContextAsync();
                    var request = context.Request;
                    var response = context.Response;

                    string authorizationCode = request.QueryString["code"];

                    string responseString = "Authorization code received. You can close this window.";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.Close();
                    _authorizationCodeCompletionSource.SetResult(authorizationCode);

                    _httpListener.Stop();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while starting HTTP server: {ex.Message}");
            }
        }

        public async Task<string> WaitForAuthorizationCodeAsync()
        {
            return await _authorizationCodeCompletionSource.Task;
        }
    }
}
