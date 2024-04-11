using CliApp.CommandLine.Context;
using CliApp.CommandLine.Services.AuthService;

namespace CliApp
{
    public class CliApp
    {
        public static async Task Main(string[] args)
        {
            new CommandInvoker(new AppState(args)).Run();

            var authService = new AuthService();

            var accessToken = await authService.GetAccessTokenAsync(Environment.GetEnvironmentVariable("GitCLI_AuthURL"))!;

            Console.WriteLine($"Access Token: {accessToken}");
            
        }
    }
}