using RestSharp;
using System;
using Newtonsoft.Json.Linq;

internal class Program
{
    private static string apiKey;
    private static void Main(string[] args)
    {
        string diffFile = "\\n--- original_file.txt\\n+++ modified_file.txt\\n@@ -1,3 +1,3 @@\\n This is the original content of the file.\\n-Here are some lines that will be removed.\\n+This line has been modified.\\n+An additional line has been added.\\n The end.";
        Console.WriteLine($"Your diff file is:\n{diffFile}\n");
        apiKey = Environment.GetEnvironmentVariable("API_KEY");
        Console.WriteLine("Summarizing your commits...");
        GetDiffSummary(diffFile);
    }

    public static void GetDiffSummary(string diffFile)
    {
        var options = new RestClientOptions("https://api.openai.com")
        {
            MaxTimeout = -1,
        };
        var client = new RestClient(options);
        var request = new RestRequest("/v1/chat/completions", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", $"Bearer {apiKey}");
        //request.AddHeader("", "");
        //string content = $"Summarize the following diff file: {diffFile}";
        request.AddHeader("Cookie", "__cf_bm=ygXwpEIMkM8GrYYDpNHIcT3nXQyY_u_Iv6qZve4R6qU-1712245781-1.0.1.1-x0b.QzRCpzXpiAui5B0Ak7w2EN5.FAF5cJTkjaLtC9wUNfhKcPgmfnToeiOrOVSps5hwSYFUG885tCndIFcuGw; _cfuvid=k17bf10kANPc4gAWlu.HtNiQreMykzWf57G2BXR_LUg-1712243648705-0.0.1.1-604800000");
        var body = @$"{{
            ""model"": ""gpt-3.5-turbo"",
            ""messages"": [
                {{
                    ""role"": ""system"",
                    ""content"": ""Summarize in detail the following diff file: {diffFile}""
                }}
            ],
            ""max_tokens"": 150
        }}";

        request.AddStringBody(body, DataFormat.Json);
        RestResponse response = client.Execute(request);
        // full reponse bellow
        //Console.WriteLine(response.Content);
        
        string content;
        // Check if the request was successful
        if (response.IsSuccessful)
        {
            // Parse the JSON response
            JObject jsonResponse = JObject.Parse(response.Content);

            // Extract the content from the response
            content = jsonResponse["choices"][0]["message"]["content"].ToString();

            // Print the extracted content
            Console.WriteLine(content + "\n");
        }
        else
        {
            content = "Error: Unable to fetch response from the API.";
            Console.WriteLine("Error: Unable to fetch response from the API.");
        }


        Console.WriteLine("Calling backend application: /api/Git...");
        var apiUrl = "https://localhost:5000";
        var userId = 1;
        var created = DateTime.UtcNow;
        var message = "string";
        var apiGetOptions = new RestClientOptions(apiUrl)
        {
            MaxTimeout = -1,
        };
        var apiGetClient = new RestClient(apiGetOptions);
        var apiGetRequest = new RestRequest("/api/Git", Method.Post);
        apiGetRequest.AddHeader("Content-Type", "application/json");
        var apiGetBody = new
        {
            userId,
            created,
            message,
            diff = diffFile,
            summary = content
        };
        apiGetRequest.AddJsonBody(apiGetBody);
        RestResponse apiGetResponse = apiGetClient.Execute(apiGetRequest);
        Console.WriteLine("/api/Git response:\n");
        Console.WriteLine(apiGetResponse.Content);
    }
}