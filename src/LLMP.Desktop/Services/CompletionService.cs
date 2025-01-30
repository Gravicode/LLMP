using LLMP.Desktop.Data;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
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
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                kernel = Kernel.CreateBuilder()
               .AddGoogleAIGeminiChatCompletion(
                   modelId: ModelId,
                   apiKey: AppConstants.PalmKey)
               .Build();
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            }
            else if (ModelId.Contains("deepseek", StringComparison.InvariantCultureIgnoreCase))
            {
                //open ai
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                kernel = Kernel.CreateBuilder()
              .AddAzureAIInferenceChatCompletion(
                  modelId: AppConstants.ModelAzureAIs.First(),
                  apiKey: AppConstants.AzureAIKey,
                  endpoint: new Uri(AppConstants.AzureAIEndpoint))
              .Build();
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
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
