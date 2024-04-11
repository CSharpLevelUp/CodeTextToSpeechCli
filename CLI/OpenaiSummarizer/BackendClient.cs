using RestSharp;
using System;

namespace OpenaiSummarizer
{
    public class BackendClient
    {
        public static void SendCommitSummary(string diffFile, string summary)
        {


            var client = new RestClient("https://localhost:5000");
            var request = new RestRequest("/api/Git", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            // N.N. add security header: something like this
            //request.AddHeader("Authentication", )

            var commitId = 0; // remove when BE auto populates it
            var userId = 1; // change to real user id
            var created = DateTime.UtcNow;
            var message = "";

            var body = new
            {   
                commitId, // remove when it auto generates on BE
                userId,
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
                Console.WriteLine("Error: Unable to send commit summary to the backend API.");
            }
        }
    }
}
