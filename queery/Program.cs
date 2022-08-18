using System;
using System.Diagnostics;
using indexer;
using SearchData = queery.SearchData;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;


namespace queery
{
    class Program
    {
        private static SearchClient _searchClient;
        
        static void Main(string[] args)
        {
            string serviceName = "azsearchteam4";
            string indexName = "azureblob-index";
            string apiKey = "CQOqmbe55SFKwh46to27bkJuuh4F5p58ioZM2QNRV1AzSeA5an9z";
            string cogServiceKey = "d9266653623b411d975d8447f62110ec";

            // Create a SearchIndexClient to send create/delete index commands
            Uri serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");
            AzureKeyCredential credential = new AzureKeyCredential(apiKey);
            SearchIndexClient adminClient = new SearchIndexClient(serviceEndpoint, credential);
            //SearchIndexerClient indexerClient = new SearchIndexerClient(serviceEndpoint, credential);

            // Create a SearchClient to load and query documents
            //SearchClient srchclient = new SearchClient(serviceEndpoint, indexName, credential);
        
            _searchClient = adminClient.GetSearchClient(indexName);
            SearchData model = new SearchData();
            RunQueryAsync(model);
        }

    

public async void  RunQueryAsync(SearchData model)
{


    var options = new SearchOptions()
    {
        IncludeTotalCount = true
    };

            // Enter Hotel property names into this list so only these values will be returned.
            // If Select is empty, all values will be returned, which can be inefficient.

            
        
            model.searchText = "New York";

            var searchResult = await _searchClient.SearchAsync<Blob>(model.searchText, options).ConfigureAwait(false); 
            model.resultList = searchResult.Value.GetResults().ToList();
           
            // For efficiency, the search call should be asynchronous, so use SearchAsync rather than Search.
            // model.resultList = await _searchClient.SearchAsync<Blob>(model.searchText, options).ConfigureAwait(false);

            
            //var list = await _searchClient.SearchAsync("New York", options).ConfigureAwait(false);

            Console.WriteLine("New York = " + model.resultList.count.toString());

    // Display the results.
    //return View("Index", model);
}
    }

}