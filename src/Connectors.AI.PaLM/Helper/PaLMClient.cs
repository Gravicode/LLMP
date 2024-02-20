using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Connectors.AI.PaLM.ChatCompletion;
using System.Linq;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Connectors.AI.PaLM.Helper
{
    public class PaLMClient
	{
        string Model { set; get; } = @"chat-bison-001";
        string ApiKey { set; get; } = "";
        const string ServiceUrl = @"https://generativelanguage.googleapis.com/v1beta2/models/[$MODEL]:generateMessage?key=[$API_KEY]";
        HttpClient Client { get; set; }
        public PaLMClient(string ApiKey, string Model="")
		{
            this.Client = new();
            if (!string.IsNullOrEmpty(Model))
            {
                this.Model = Model;
            }
            this.ApiKey = ApiKey;
		}

        public virtual async Task<string> GetMessageAsync(PaLMChatHistory history, PromptExecutionSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                OpenAIPromptExecutionSettings oaisetting = OpenAIPromptExecutionSettings.FromExecutionSettings(settings);
                var context = history.Where(x => x.Role == AuthorRole.System).FirstOrDefault();
                var hist = new List<MessageRequest>();
                history.ToList().ForEach(x => { if (x.Role != AuthorRole.System) { hist.Add(new MessageRequest() { content = x.Content }); } });
                var json = new RequestPaLMChat() { candidate_count = 1, prompt = new PaLMPrompt() { context = context?.Content, examples = new(), messages = hist }, temperature = oaisetting.Temperature, top_p = oaisetting.TopP, top_k = 40  };
                var url = ServiceUrl.Replace("[$MODEL]", this.Model).Replace("[$API_KEY]", this.ApiKey);
                var res = await Client.PostAsync(url, new StringContent(JsonSerializer.Serialize(json), System.Text.Encoding.UTF8, "application/json"), cancellationToken);
                if (res.IsSuccessStatusCode)
                {
                    var content =  await res.Content.ReadAsStringAsync();
                    var obj = JsonSerializer.Deserialize<ResponsePaLMChat>(content);
                    
                    var desc = obj?.candidates?.First().content;
                    return string.IsNullOrEmpty(desc) ? "PaLM refuse to answer" : desc;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            return string.Empty;
        }


    }
    #region model response
    public class CandidateChat
    {
        public string author { get; set; }
        public string content { get; set; }
    }

    public class MessageResponse
    {
        public string author { get; set; }
        public string content { get; set; }
    }

    public class ResponsePaLMChat
    {
        public List<CandidateChat> candidates { get; set; }
        public List<MessageResponse> messages { get; set; }
    }
    #endregion
    #region models request
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ExampleChat
    {
        public InputText input { get; set; }
        public OutputText output { get; set; }
    }

    public class InputText
    {
        public string content { get; set; }
    }

    public class MessageRequest
    {
        public string content { get; set; }
    }

    public class OutputText
    {
        public string content { get; set; }
    }

    public class PaLMPrompt
    {
        public string context { get; set; }
        public List<ExampleChat> examples { get; set; }
        public List<MessageRequest> messages { get; set; }
    }

    public class RequestPaLMChat
    {
        public PaLMPrompt prompt { get; set; }
        public double temperature { get; set; }
        public int top_k { get; set; }
        public double top_p { get; set; }
        public int candidate_count { get; set; }
    }


    #endregion
}

