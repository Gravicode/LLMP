using LLMP.Desktop.Data;
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
        bool HasDoc { set; get; } = false;
        bool IsReady { set; get; } = false;
        MemoryServerless memory;
        public ChatDocService()
        {

        }
        public async Task<string> Chat(string Message)
        {
            if (!HasDoc)
            {
                return "please index at least one document.";
            }
            var result = await memory.AskAsync(Message, minRelevance: MinRelevance);
            return result.Result;
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
            memory = new KernelMemoryBuilder()
    .WithOpenAIDefaults(AppConstants.OpenAIKey, AppConstants.OpenAIOrg)
    //.WithAzureOpenAITextGeneration(azureOpenAITextConfig, new DefaultGPTTokenizer())
    //.WithAzureOpenAITextEmbeddingGeneration(azureOpenAIEmbeddingConfig, new DefaultGPTTokenizer())
    .Build<MemoryServerless>();
            HasDoc = false;
            IsReady = true;


        }
    }
}
