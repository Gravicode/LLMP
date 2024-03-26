using DocumentFormat.OpenXml.Spreadsheet;
using LLMP.Desktop.Data;
using LLMP.Models;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI.OpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMP.Desktop.Services
{
    public class ChatDocService
    {
        public double MinRelevance { set; get; } = 0.76;
        bool HasDoc { set; get; } = true;
        bool IsReady { set; get; } = false;
        MemoryServerless memory;
        public ChatDocService()
        {

        }

        public async Task<bool> Reset()
        {
            try
            {
                var indexes = await memory.ListIndexesAsync();

                if (indexes.Any(x => x.Name == "default"))
                {
                    await memory.DeleteIndexAsync();                 
                    HasDoc = false;
                    return true;
                }
             
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync();
            }
            return false;
        
        }
        public async Task<string> Chat(string Message)
        {
            if (!HasDoc)
            {
                return "please index at least one document.";
            }
            var result = await memory.AskAsync(Message, minRelevance: MinRelevance);
            var Hasil = result.Result+Environment.NewLine;
            Hasil += result.RelevantSources.Any() ? "\r\nReferensi:\r\n" : "";
            foreach (var x in result.RelevantSources)
            {
                Hasil+=($"  - {x.SourceName}  - {x.Link} [{x.Partitions.First().LastUpdate:D}]");
            }
            return Hasil;
        }
        public async Task ImportDoc(string Filename)
        {
            FileInfo info = new FileInfo(Filename);
            if (info.Exists && IsReady)
            {
                if (info.Extension == ".csv")
                {
                    var content = File.ReadAllText(info.FullName);
                    await memory.ImportTextAsync(content, tags: new() { { "file", info.Name }, { "date", info.CreationTime.ToString() } });

                }
                else
                {
                    // Import a file
                    await memory.ImportDocumentAsync(Filename, tags: new() { { "file", info.Name }, { "date", info.CreationTime.ToString() } });
                }

                HasDoc = true;
            }
        }
        public async Task Setup()
        {
            //var azureOpenAITextConfig = new AzureOpenAIConfig() { APIKey = AppConstants.OpenAIKey, Auth = AzureOpenAIConfig.AuthTypes.APIKey };
            //var azureOpenAIEmbeddingConfig = new AzureOpenAIConfig() { APIKey = AppConstants.OpenAIKey, Deployment = AppConstants.EmbeddingModel };
            var recognizerConfig = new AzureAIDocIntelConfig() { APIKey = AppConstants.AzureFormApiKey, Auth= AzureAIDocIntelConfig.AuthTypes.APIKey, Endpoint = AppConstants.AzureFormEndpoint };
            var searchConfig = new AzureAISearchConfig() { APIKey = AppConstants.AzureSearchApiKey, Auth=  AzureAISearchConfig.AuthTypes.APIKey, Endpoint = AppConstants.AzureSearchEndpoint };
            memory = new KernelMemoryBuilder()
    .WithOpenAIDefaults(AppConstants.OpenAIKey, AppConstants.OpenAIOrg)
    //.WithAzureOpenAITextGeneration(azureOpenAITextConfig, new DefaultGPTTokenizer())
    //.WithAzureOpenAITextEmbeddingGeneration(azureOpenAIEmbeddingConfig, new DefaultGPTTokenizer())
    .WithAzureAIDocIntel(recognizerConfig)
    .WithAzureAISearchMemoryDb(searchConfig)
    .Build<MemoryServerless>();
            HasDoc = true;
            IsReady = true;


        }
    }
}
