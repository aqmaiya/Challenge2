using System;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

namespace searchindex

{
    class Program
    {
        static void Main(string[] args)
        {

            string serviceName = "azsearchteam4";
            string apiKey = "CQOqmbe55SFKwh46to27bkJuuh4F5p58ioZM2QNRV1AzSeA5an9z";
            string indexName = "azureblob-index";

            // Create a SearchIndexClient to send create/delete index commands
            Uri serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");
            AzureKeyCredential credential = new AzureKeyCredential(apiKey);
            SearchIndexClient adminClient = new SearchIndexClient(serviceEndpoint, credential);

            // Create a SearchClient to load and query documents
            SearchClient srchclient = new SearchClient(serviceEndpoint, indexName, credential);

            Console.WriteLine("Starting queries...\n");
            RunQueries(srchclient);

            // End the program
            Console.WriteLine("{0}", "Complete. Press any key to end this program...\n");
            Console.ReadKey();

        }

        // Run queries, use WriteDocuments to print output
        private static void RunQueries(SearchClient srchclient)
        {
            SearchOptions options;
            SearchResults<Blob> response;

            // Query 1
            Console.WriteLine("Query #1: Search on empty term '*' to return all documents, showing a subset of fields...\n");

            options = new SearchOptions()
            {
                IncludeTotalCount = true,
                Filter = "",
                OrderBy = { "" }
            };

            //options.Select.Add("URL");
            options.Select.Add("file_name");
            //options.Select.Add("Rating");

            response = srchclient.Search<Blob>("*", options);
            WriteDocuments(response);

        }

        // Write search results to console
        private static void WriteDocuments(SearchResults<Blob> searchResults)
        {
            foreach (SearchResult<Blob> result in searchResults.GetResults())
            {
                Console.WriteLine(result.Document);
            }

            Console.WriteLine();
        }
    }
}