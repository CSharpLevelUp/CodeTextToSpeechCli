using OpenaiSummarizer;
using System;
internal class Program
{
    private static void Main(string[] args)
    {
        string diffFile = args[0];
        Console.WriteLine($"Your diff file is:\n{diffFile}\n");
        string apiKey = Environment.GetEnvironmentVariable("API_KEY");

        Console.WriteLine("Calling Openai to summarize the diff file...");
        string summary = OpenAIHelper.GetDiffSummary(diffFile, apiKey);

        if (summary != null)
        {
            Console.WriteLine("Summary: \n" + summary + "\n");

            Console.WriteLine("Calling backend application: /api/Git...");
            BackendClient.SendCommitSummary(diffFile, summary);
        }
        else
        {
            Console.WriteLine("Error: Unable to fetch summary from OpenAI API.");
        }
    }
}