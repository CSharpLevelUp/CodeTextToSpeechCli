using CliApp.CommandLine.DataClasses;
using OpenaiSummarizer;
using System;
public class Program
{
    public static void Main(string[] args)
    {
        string diffFile = args[0];
        string access_token = args[1];
        Console.WriteLine($"Your diff file is:\n{diffFile}\n");
        Console.WriteLine($"access_token:\n{access_token}\n");
        string apiKey = Environment.GetEnvironmentVariable("CLI_OPENAI_KEY");

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