
using Azure.Search.Documents.Models;
using indexer;

namespace queery
{
    public class SearchData
    {
        public string searchText { get; set; }
        public SearchResults<Blob> resultList { get; set; }
        
    }
}
