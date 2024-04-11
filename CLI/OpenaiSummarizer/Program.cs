using OpenaiSummarizer;
using System;
internal class Program
{
    private static void Main(string[] args)
    {
        string diffFile = "\\n--- original_file.txt\\n+++ modified_file.txt\\n@@ -1,3 +1,3 @@\\n This is the original content of the file.\\n-Here are some lines that will be removed.\\n+This line has been modified.\\n+An additional line has been added.\\n The end.";
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