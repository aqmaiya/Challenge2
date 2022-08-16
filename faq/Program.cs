using Azure;
using Azure.AI.Language.QuestionAnswering;
using System;

namespace question_answering
{
    class Program
    {
        static void Main(string[] args)
        {

            Uri endpoint = new Uri("https://langresteam4.cognitiveservices.azure.com/");
            AzureKeyCredential credential = new AzureKeyCredential("fda2224625a34ed3b3cad0fbc9a511ee");
            string projectName = "Chat02-FAQ";
            string deploymentName = "production";

            string question = "How can I cancel my hotel reservation?";

            QuestionAnsweringClient client = new QuestionAnsweringClient(endpoint, credential);
            QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);

            Response<AnswersResult> response = client.GetAnswers(question, project);

            foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
            {
                Console.WriteLine($"Q:{question}");
                Console.WriteLine($"A:{answer.Answer}");
            }
        }
    }
}