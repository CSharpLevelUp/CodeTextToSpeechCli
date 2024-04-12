using System;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace OpenaiSummarizer
{
    public class OpenAIHelper
    {
        public static string GetDiffSummary(string diffFile, string apiKey)
        {
            var client = new RestClient("https://api.openai.com/v1");
            var request = new RestRequest("/chat/completions", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {apiKey}");

            dynamic requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = $"Summarize in detail the following diff file: {diffFile}"
                    }
                },
                max_tokens = 1000
            };
            string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            RestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                JObject jsonResponse = JObject.Parse(response.Content);
                return jsonResponse["choices"][0]["message"]["content"].ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
