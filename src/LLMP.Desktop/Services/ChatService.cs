using LLMP.Desktop.Data;
using LLMP.Models;
using Azure.AI.Inference;
//using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace LLMP.Desktop.Services
{
    public class ChatService
    {
        public OpenAIPromptExecutionSettings Setting { set; get; }
        bool IsReady { set; get; }
        ChatHistory chatHistory { set; get; }
        IChatCompletionService chatCompletionService { set; get; }
        string SystemMessage { set; get; } = "You are a librarian, expert about books";
        public ChatService()
        {
            Setting = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 2000,
                Temperature = 0.7,
                TopP = 0.5
            };
        }

        public void SetupChat(string ModelId, string SystemMessage = null)
        {
            if (!string.IsNullOrEmpty(SystemMessage))
            {
                this.SystemMessage = SystemMessage;
            }
            if (ModelId.Contains("bison") || ModelId.Contains("gecko") || ModelId.Contains("gemini") || ModelId.Contains("embedding"))
            {
                //palm
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                chatCompletionService = new GoogleAIGeminiChatCompletionService(ModelId, AppConstants.PalmKey);
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            }
            else if (ModelId.Contains("deepseek",StringComparison.InvariantCultureIgnoreCase))
            {
                //open ai
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                chatCompletionService = new Microsoft.Extensions.AI.AzureAIInferenceChatClient(new Azure.AI.Inference.ChatCompletionsClient(new Uri(AppConstants.AzureAIEndpoint),new Azure.AzureKeyCredential(AppConstants.AzureAIKey)),modelId: AppConstants.ModelAzureAIs.First()).AsChatCompletionService();
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            }
            else
            {
                //open ai
                chatCompletionService = new OpenAIChatCompletionService(modelId: ModelId, openAIClient: new OpenAI.OpenAIClient(AppConstants.OpenAIKey));
            }

            chatHistory = new ChatHistory(this.SystemMessage);
            IsReady = true;
        }

        public void Load(RagData data)
        {
            this.Setting.Temperature = data.Setting.Temperature;
            this.Setting.TopP = data.Setting.TopP;
            this.Setting.MaxTokens = data.Setting.MaxToken;
            this.SystemMessage = data.SystemMessage;
            var ModelId = data.ModelId;
            if (ModelId.Contains("bison") || ModelId.Contains("gecko") || ModelId.Contains("gemini") || ModelId.Contains("embedding"))
            {
                //palm
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                chatCompletionService = new GoogleAIGeminiChatCompletionService(ModelId, AppConstants.PalmKey);
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            }
            else
            {
                //open ai
                chatCompletionService = new OpenAIChatCompletionService(ModelId, AppConstants.OpenAIKey);
            }

            chatHistory = new ChatHistory(this.SystemMessage);
            foreach (var item in data.Items)
            {
                if (string.IsNullOrEmpty(item.ImageUrl))
                {
                    chatHistory.AddUserMessage(item.Question);
                    chatHistory.AddAssistantMessage(item.Answer);

                }
                else
                {
                    chatHistory.AddUserMessage(new ChatMessageContentItemCollection
        {
            new TextContent(item.Question),
            new ImageContent(new Uri(item.ImageUrl))
        });
                    chatHistory.AddAssistantMessage(item.Answer);
                }

            }
            IsReady = true;
        }

            /// <summary>
            /// Outputs the last message of the chat history
            /// </summary>
            public async Task<string?> Chat(string UserMessage, string ImageUrl = "")
            {
                if (!IsReady) SetupChat(AppConstants.DefaultModel);
                // First user message
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    chatHistory.AddUserMessage(UserMessage);
                }
                else
                {
                    chatHistory.AddUserMessage(new ChatMessageContentItemCollection
        {
            new TextContent(UserMessage),
            new ImageContent(new Uri(ImageUrl))
        });
                }
                var message = chatHistory.Last();

                Console.WriteLine($"{message.Role}: {message.Content}");
                Console.WriteLine("------------------------");

                // First bot assistant message


                var reply = await chatCompletionService.GetChatMessageContentAsync(chatHistory, Setting);
                chatHistory.Add(reply);

                message = chatHistory.Last();

                Console.WriteLine($"{message.Role}: {message.Content}");
                Console.WriteLine("------------------------");

                return message.Content;
            }
        }
    }
