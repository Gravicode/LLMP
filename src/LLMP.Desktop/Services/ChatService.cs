using LLMP.Desktop.Data;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.PaLM.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void SetupChat(string ModelId, string SystemMessage=null)
        {
            if (!string.IsNullOrEmpty(SystemMessage))
            {
                this.SystemMessage = SystemMessage;
            }
            if(ModelId.Contains("bison") || ModelId.Contains("gecko") || ModelId.Contains("gemini")|| ModelId.Contains("embedding"))
            {
                //palm
                chatCompletionService = new PaLMChatCompletion(ModelId, AppConstants.PalmKey);
            }
            else
            {
                //open ai
                chatCompletionService = new OpenAIChatCompletionService(ModelId, AppConstants.OpenAIKey);
            }
          
            chatHistory = new ChatHistory(this.SystemMessage);
            IsReady = true;
        }

        /// <summary>
        /// Outputs the last message of the chat history
        /// </summary>
        public async Task<string?> Chat(string UserMessage,string ImageUrl = "")
        {
            if(!IsReady) SetupChat(AppConstants.DefaultModel);
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
          

            var reply = await chatCompletionService.GetChatMessageContentAsync(chatHistory,Setting);
            chatHistory.Add(reply);
          
            message = chatHistory.Last();

            Console.WriteLine($"{message.Role}: {message.Content}");
            Console.WriteLine("------------------------");

            return message.Content;
        }
    }
}
