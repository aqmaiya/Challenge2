using System;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;


namespace indexer
{
    class Program
    {
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
            SearchIndexerClient indexerClient = new SearchIndexerClient(serviceEndpoint, credential);

            // Create a SearchClient to load and query documents
            SearchClient srchclient = new SearchClient(serviceEndpoint, indexName, credential);

            // Delete index if it exists
            Console.WriteLine("{0}", "Deleting index...\n");
            DeleteIndexIfExists(indexName, adminClient);

            // Create index
            Console.WriteLine("{0}", "Creating index...\n");
            SearchIndex index = CreateIndex(indexName, adminClient);

            // Create or Update the data source
            Console.WriteLine("Creating or updating the data source...");
            SearchIndexerDataSourceConnection dataSource = CreateOrUpdateDataSource(indexerClient);

            // Create the skills
            Console.WriteLine("Creating the skills...");
            //OcrSkill ocrSkill = CreateOcrSkill();
            //MergeSkill mergeSkill = CreateMergeSkill();
            //EntityRecognitionSkill entityRecognitionSkill = CreateEntityRecognitionSkill();
            //LanguageDetectionSkill languageDetectionSkill = CreateLanguageDetectionSkill();
            //SplitSkill splitSkill = CreateSplitSkill();
            SentimentSkill createSentimentSkill1 = createSentimentSkill();
            KeyPhraseExtractionSkill keyPhraseExtractionSkill = CreateKeyPhraseExtractionSkill();
            EntityRecognitionSkill  createEntityRecognitionSkill = CreateEntityRecognitionSkill();

// Create the skillset
Console.WriteLine("Creating or updating the skillset...");
List<SearchIndexerSkill> skills = new List<SearchIndexerSkill>();
//skills.Add(ocrSkill);
//skills.Add(mergeSkill);
//skills.Add(languageDetectionSkill);
//skills.Add(splitSkill);
//skills.Add(entityRecognitionSkill);
skills.Add(createSentimentSkill1);
skills.Add(keyPhraseExtractionSkill);
skills.Add(createEntityRecognitionSkill);


SearchIndexerSkillset skillset = CreateOrUpdateDemoSkillSet(indexerClient, skills, cogServiceKey);

            // Create the indexer, map fields, and execute transformations
            Console.WriteLine("Creating the indexer and executing the pipeline...");
            SearchIndexer indexer = CreateIndexer(indexerClient, dataSource, skillset, index);

            // Key phrase extration skill
             //KeyPhraseExtractionSkill skillset =  CreateKeyPhraseExtractionSkill();
            //SearchClient ingesterClient = adminClient.GetSearchClient(indexName);

            // Load documents
            //Console.WriteLine("{0}", "Uploading documents...\n");
            //UploadDocuments(ingesterClient);

            // Wait 2 secondsfor indexing to complete before starting queries (for demo and console-app purposes only)
            //Console.WriteLine("Waiting for indexing...\n");
            //System.Threading.Thread.Sleep(2000);

            // Call the RunQueries method to invoke a series of queries
            //Console.WriteLine("Starting queries...\n");
            //RunQueries(srchclient);

            // End the program
            Console.WriteLine("{0}", "Complete. Press any key to end this program...\n");
            Console.ReadKey();
        }

        // Delete the hotels-quickstart index to reuse its name
        private static void DeleteIndexIfExists(string indexName, SearchIndexClient adminClient)
        {
            adminClient.GetIndexNames();
            {
                adminClient.DeleteIndex(indexName);
            }
        }

        // Create hotels-quickstart index
        private static SearchIndex CreateIndex(string indexName, SearchIndexClient adminClient)
        {
            FieldBuilder fieldBuilder = new FieldBuilder();
            var searchFields = fieldBuilder.Build(typeof(Blob));

            var definition = new SearchIndex(indexName, searchFields);

            //var suggester = new SearchSuggester("sg", new[] { "FileName" });
            //definition.Suggesters.Add(suggester);

            return adminClient.CreateOrUpdateIndex(definition);
        }

        private static SearchIndexerDataSourceConnection CreateOrUpdateDataSource(SearchIndexerClient indexerClient)
        {
            SearchIndexerDataSourceConnection dataSource = new SearchIndexerDataSourceConnection(
                name: "websitedocs",
                type: SearchIndexerDataSourceType.AzureBlob,
                connectionString: "DefaultEndpointsProtocol=https;AccountName=challenge02sa;AccountKey=khnAThBXT5D8/ErFh+tt2mqEkdyK8fYn/Iy0Qdh5uJH36P7Du6O9+dgGGbYVdkSGN28zvV5MtcuP+AStbyYMxQ==;EndpointSuffix=core.windows.net",
                container: new SearchIndexerDataContainer("websitedocs"))
            {
                Description = "Website docs to demonstrate cognitive search capabilities."
            };

            // The data source does not need to be deleted if it was already created
            // since we are using the CreateOrUpdate method
            try
            {
                indexerClient.CreateOrUpdateDataSourceConnection(dataSource);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to create or update the data source\n Exception message: {0}\n", ex.Message);
            }

            return dataSource;
        }

        private static SearchIndexer CreateIndexer(SearchIndexerClient indexerClient, SearchIndexerDataSourceConnection dataSource, SearchIndexerSkillset skillset, SearchIndex index)
        {
            IndexingParameters indexingParameters = new IndexingParameters()
            {
                MaxFailedItems = -1,
                MaxFailedItemsPerBatch = -1,
            };
            indexingParameters.Configuration.Add("dataToExtract", "contentAndMetadata");
            //indexingParameters.Configuration.Add("imageAction", "generateNormalizedImages");

            SearchIndexer indexer = new SearchIndexer("azureblob-indexer", dataSource.Name, index.Name)
            {
                Description = "Blob Indexer",
                SkillsetName = skillset.Name,
                Parameters = indexingParameters
            };

            FieldMappingFunction mappingFunction = new FieldMappingFunction("base64Encode");
            mappingFunction.Parameters.Add("useHttpServerUtilityUrlTokenEncode", true);

            indexer.FieldMappings.Add(new FieldMapping("metadata_storage_path")
            {
                TargetFieldName = "id",
                MappingFunction = mappingFunction
            });
            indexer.FieldMappings.Add(new FieldMapping("metadata_storage_path")
            {
                TargetFieldName = "url"
            });
            indexer.FieldMappings.Add(new FieldMapping("metadata_storage_name")
            {
                TargetFieldName = "file_name"
            });
            indexer.FieldMappings.Add(new FieldMapping("content")
            {
                TargetFieldName = "content"
            });
            indexer.FieldMappings.Add(new FieldMapping("metadata_storage_name")
            {
                TargetFieldName = "metadata_storage_name"
            });
            indexer.FieldMappings.Add(new FieldMapping("metadata_storage_size")
            {
                TargetFieldName = "metadata_storage_size"
            });
            indexer.FieldMappings.Add(new FieldMapping("metadata_creation_date")
            {
                TargetFieldName = "metadata_creation_date"
            });
            indexer.OutputFieldMappings.Add(new FieldMapping("/document/content/keyPhrases/*")
            {
                TargetFieldName = "keyPhrases"
            });
            indexer.OutputFieldMappings.Add(new FieldMapping("/document/content/score")
            {
                TargetFieldName = "score"
            });
            indexer.OutputFieldMappings.Add(new FieldMapping("/document/content/persons/*")
            {
                TargetFieldName = "persons"
            });
            indexer.OutputFieldMappings.Add(new FieldMapping("/document/content/locations/*")
            {
                TargetFieldName = "locations"
            });
            indexer.OutputFieldMappings.Add(new FieldMapping("/document/content/urls/*")
            {
                TargetFieldName = "urls"
            });



            try
            {
                indexerClient.GetIndexer(indexer.Name);
                indexerClient.DeleteIndexer(indexer.Name);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                //if the specified indexer not exist, 404 will be thrown.
            }

            try
            {
                indexerClient.CreateIndexer(indexer);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("Failed to create the indexer\n Exception message: {0}\n", ex.Message);
            }

            return indexer;
        }

        //Key phrase extraction skill
private static KeyPhraseExtractionSkill CreateKeyPhraseExtractionSkill()
{
    List<InputFieldMappingEntry> inputMappings = new List<InputFieldMappingEntry>();
    inputMappings.Add(new InputFieldMappingEntry("text")
    {
       Source = "/document/content"
        

    });
 /*   inputMappings.Add(new InputFieldMappingEntry("languageCode")
    {
        Source = "/document/language"
    });
*/
    List<OutputFieldMappingEntry> outputMappings = new List<OutputFieldMappingEntry>();
    outputMappings.Add(new OutputFieldMappingEntry("keyPhrases")
    {
        TargetName = "keyPhrases"
    });

    KeyPhraseExtractionSkill keyPhraseExtractionSkill = new KeyPhraseExtractionSkill(inputMappings, outputMappings)
    {
        Description = "Extract the key phrases",
        Context = "/document/content",
        DefaultLanguageCode = KeyPhraseExtractionSkillLanguage.En
    };

    return keyPhraseExtractionSkill;
}

        private static SentimentSkill createSentimentSkill()
        {
            List<InputFieldMappingEntry> inputMappings = new List<InputFieldMappingEntry>();
            inputMappings.Add(new InputFieldMappingEntry("text")
            {
                Source = "/document/content"


            });
     
            List<OutputFieldMappingEntry> outputMappings = new List<OutputFieldMappingEntry>();
            /*     outputMappings.Add(new OutputFieldMappingEntry("sentiment")
                 {
                     TargetName = "sentiment"
                 });
                 outputMappings.Add(new OutputFieldMappingEntry("confidenceScores")
                 {
                     TargetName = "confidenceScores"
                 });
                 outputMappings.Add(new OutputFieldMappingEntry("sentences")
                 {
                     TargetName = "sentences"
                 });
       */
            outputMappings.Add(new OutputFieldMappingEntry("score")
            {
                TargetName = "score"
            });


            SentimentSkill createSentimentSkill = new SentimentSkill(inputMappings, outputMappings)
            {
                Description = "Extract the key phrases",
                Context = "/document/content",
                DefaultLanguageCode = SentimentSkillLanguage.En, 
                //ModelVersion = SentimentSkill.SkillVersion.V3.ToString()
            };

            return createSentimentSkill;
        }

        private static EntityRecognitionSkill CreateEntityRecognitionSkill()
        {
            List<InputFieldMappingEntry> inputMappings = new List<InputFieldMappingEntry>();
            inputMappings.Add(new InputFieldMappingEntry("text")
            {
                Source = "/document/content"
            });

            List<OutputFieldMappingEntry> outputMappings = new List<OutputFieldMappingEntry>();

            outputMappings.Add(new OutputFieldMappingEntry("persons")
            {
                TargetName = "persons"
            });
            outputMappings.Add(new OutputFieldMappingEntry("urls")
            {
                TargetName = "urls"
            });
            outputMappings.Add(new OutputFieldMappingEntry("locations")
            {
                TargetName = "locations"
            });

  

            EntityRecognitionSkill entityRecognitionSkill = new EntityRecognitionSkill(inputMappings, outputMappings)
            {
                Description = "Recognize organizations",
                Context = "/document/content",
                DefaultLanguageCode = EntityRecognitionSkillLanguage.En
            };
            entityRecognitionSkill.Categories.Add(EntityCategory.Organization);

            return entityRecognitionSkill;
        }

private static SearchIndexerSkillset CreateOrUpdateDemoSkillSet(SearchIndexerClient indexerClient, IList<SearchIndexerSkill> skills,string cognitiveServicesKey)
{
    SearchIndexerSkillset skillset = new SearchIndexerSkillset("demoskillset", skills)
    {
        Description = "Demo skillset",
        CognitiveServicesAccount = new CognitiveServicesAccountKey(cognitiveServicesKey)
    };

    // Create the skillset in your search service.
    // The skillset does not need to be deleted if it was already created
    // since we are using the CreateOrUpdate method
    try
    {
        indexerClient.CreateOrUpdateSkillset(skillset);
    }
    catch (RequestFailedException ex)
    {
        Console.WriteLine("Failed to create the skillset\n Exception message: {0}\n", ex.Message);
        //ExitProgram("Cannot continue without a skillset");
    }

    return skillset;
}

        private static SplitSkill CreateSplitSkill()
        {
            List<InputFieldMappingEntry> inputMappings = new List<InputFieldMappingEntry>();
            inputMappings.Add(new InputFieldMappingEntry("text")
            {
                Source = "/document/merged_text"
            });
            inputMappings.Add(new InputFieldMappingEntry("languageCode")
            {
                Source = "/document/languageCode"
            });

            List<OutputFieldMappingEntry> outputMappings = new List<OutputFieldMappingEntry>();
            outputMappings.Add(new OutputFieldMappingEntry("textItems")
            {
                TargetName = "pages",
            });

            SplitSkill splitSkill = new SplitSkill(inputMappings, outputMappings)
            {
                Description = "Split content into pages",
                Context = "/document",
                TextSplitMode = TextSplitMode.Pages,
                MaximumPageLength = 4000,
                DefaultLanguageCode = SplitSkillLanguage.En
            };

            return splitSkill;
        }
    }
}