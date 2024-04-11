using RestSharp;
using System;

namespace OpenaiSummarizer
{
    public class BackendClient
    {
        public static void SendCommitSummary(string diffFile, string summary, string accessToken)
        {
            var client = new RestClient("https://localhost:5000");
            var request = new RestRequest("/api/Git", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + accessToken);

            var created = DateTime.UtcNow;
            var message = "";

            var body = new
            {   
                created,
                message,
                diff = diffFile,
                summary
            };

            request.AddJsonBody(body);
            RestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine("/api/Git response:");
                Console.WriteLine(response.Content);
            }
            else
            {
                throw new BackendClientException("Error: Unable to send commit summary to the backend API.");
            }
        }

        public class BackendClientException(string message): Exception(message)
        {}
    }
}
