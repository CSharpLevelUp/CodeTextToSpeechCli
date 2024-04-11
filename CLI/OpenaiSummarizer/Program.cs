using OpenaiSummarizer;
using System;
public class Program
{
    public static void Main(string[] args)
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
            throw new Exception("Error: Unable to fetch summary from OpenAI API.");
        }
    }
}