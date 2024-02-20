using LLMP.Desktop.Data;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace LLMP.Desktop.Services
{
    public class CompletionService
    {
        bool IsReady { set; get; }
        public OpenAIPromptExecutionSettings Setting { set; get; }
        Kernel kernel { set; get; }
        public string SelectedModel { set; get; }
        public CompletionService()
        {
            this.SelectedModel = AppConstants.ModelOpenAIs.First();
            Setting = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 2000,
                Temperature = 0.7,
                TopP = 0.5
            };
        }

        public async Task<string?> ExecutePrompt(string Message)
        {
            if (!IsReady) Setup(this.SelectedModel);
            var fixedFunction = kernel.CreateFunctionFromPrompt(Message, Setting);
            var result = await kernel.InvokeAsync(fixedFunction);
            var Result = result.GetValue<string>();
            return Result;
        }

        public void Setup(string ModelId)
        {
            this.SelectedModel = ModelId;
            if (this.SelectedModel.Contains("bison") || this.SelectedModel.Contains("gecko") || ModelId.Contains("gemini") || ModelId.Contains("embedding"))
            {
                kernel = Kernel.CreateBuilder()
               .AddPaLMChatCompletion(
                   modelId: ModelId,
                   apiKey: AppConstants.PalmKey)
               .Build();
            }
            else
            {
                kernel = Kernel.CreateBuilder()
               .AddOpenAIChatCompletion(
                   modelId: ModelId,
                   apiKey: AppConstants.OpenAIKey,
                   orgId: AppConstants.OpenAIOrg)
               .Build();
            }
            IsReady = true;
        }
    }
}
