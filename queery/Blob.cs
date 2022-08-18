using System;
using System.Text.Json.Serialization;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace indexer
{
    public partial class Blob
    {
        [SimpleField(IsKey = true, IsSortable = true, IsFilterable = true)]
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [SearchableField(IsSortable = true, IsFilterable = true)]
        [JsonPropertyName("url")]
        public string URL { get; set; }

        [SearchableField(IsSortable = true, IsFilterable = true)]
        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [SearchableField()]
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [SearchableField()]
        [JsonPropertyName("metadata_storage_name")]
        public string MetadataStorageName { get; set; }

        [SearchableField()]
        [JsonPropertyName("metadata_storage_size")]
        public string MetadataStorageSize { get; set; }

        [SearchableField()]
        [JsonPropertyName("metadata_creation_date")]
        public string MetadataCreationDate { get; set; }

        [SearchableField]
        [JsonPropertyName("keyPhrases")]
        public string[] KeyPhrases { get; set; }

        [SearchableField]
        [JsonPropertyName("persons")]
        public string[] Persons { get; set; }

        [SearchableField]
        [JsonPropertyName("urls")]
        public string[] Urls { get; set; }

        [SearchableField]
        [JsonPropertyName("locations")]
        public string[] Locations { get; set; }

        [SearchableField]
        [JsonPropertyName("score")]
        public string Score { get; set; }

       /* [SearchableField]
        [JsonPropertyName("sentiment")]
        public string Sentiment { get; set; }
        
       
        [JsonPropertyName("confidenceScores")]
        public ComplexField ConfidenceScores { get; set; }
        

        [JsonPropertyName("sentences")]
        public ComplexField Sentences { get; set; }
    */
    }
}