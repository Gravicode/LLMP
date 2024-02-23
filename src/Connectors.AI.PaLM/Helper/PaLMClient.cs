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
using Microsoft.SemanticKernel.Connectors.AI.PaLM.TextCompletion;
using System.IO;

namespace Connectors.AI.PaLM.Helper
{
    public class PaLMClient
    {
        //string Model { set; get; } = @"chat-bison-001";
        string Model { set; get; } = @"gemini-pro";
        string ApiKey { set; get; } = "";
        //const string ServiceUrl = @"https://generativelanguage.googleapis.com/v1beta2/models/[$MODEL]:generateMessage?key=[$API_KEY]";
        const string ServiceUrl = @"https://generativelanguage.googleapis.com/v1beta/models/[$MODEL]:generateContent?key=[$API_KEY]";
        HttpClient Client { get; set; }
        public PaLMClient(string ApiKey, string Model = "")
        {
            this.Client = new();
            if (!string.IsNullOrEmpty(Model))
            {
                this.Model = Model;
            }
            this.ApiKey = ApiKey;
        }
        /*
        public virtual async Task<string> GetMessageAsync(PaLMChatHistory history, PromptExecutionSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                OpenAIPromptExecutionSettings oaisetting = OpenAIPromptExecutionSettings.FromExecutionSettings(settings);
                var context = history.Where(x => x.Role == AuthorRole.System).FirstOrDefault();
                var hist = new List<MessageRequest>();
                history.ToList().ForEach(x => { if (x.Role != AuthorRole.System) { hist.Add(new MessageRequest() { content = x.Content }); } });
                var json = new RequestPaLMChat() { candidate_count = 1, prompt = new PaLMPrompt() { context = context?.Content, examples = new(), messages = hist }, temperature = oaisetting.Temperature, top_p = oaisetting.TopP, top_k = 1 };
                var url = ServiceUrl.Replace("[$MODEL]", this.Model).Replace("[$API_KEY]", this.ApiKey);
                var res = await Client.PostAsync(url, new StringContent(JsonSerializer.Serialize(json), System.Text.Encoding.UTF8, "application/json"), cancellationToken);
                if (res.IsSuccessStatusCode)
                {
                    var content = await res.Content.ReadAsStringAsync();
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
        */
       
        public virtual async Task<string> GetMessageAsync(PaLMChatHistory history, PromptExecutionSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                OpenAIPromptExecutionSettings oaisetting = OpenAIPromptExecutionSettings.FromExecutionSettings(settings);
                var context = history.Where(x => x.Role == AuthorRole.System).FirstOrDefault();
                var hist = new List<ContentChat>();
                history.ToList().ForEach(async x => {
                    if (x.Role != AuthorRole.System)
                    {
                        InlineData inlinedata = null;
                        if(x.Items.Any(x=>x is ImageContent))
                        {
                            var imgContent = x.Items.Where(x => x is ImageContent).FirstOrDefault() as ImageContent;
                            var url = imgContent.Uri.ToString();
                            var mime = $"image/{Path.GetExtension(url).Replace(".","")}";
                            var bytes = await Client.GetByteArrayAsync(url);                            
                            string base64img = Convert.ToBase64String(bytes);
                            inlinedata = new InlineData() { mime_type = mime, data = base64img };
                            
                        }
                        hist.Add(new ContentChat() { role = x.Role == AuthorRole.Assistant ? "model" : "user", parts = new PartContent[] { new PartContent() { text = x.Content, inline_data = inlinedata ?? new() } } });
                    } 
                });
                var json = new RequestPaLMChat() { contents = hist.ToArray(), generationConfig = new Generationconfig() { temperature = (float)oaisetting.Temperature, topP = (float)oaisetting.TopP, topK = 1, maxOutputTokens = oaisetting.MaxTokens.Value, stopSequences = (oaisetting.StopSequences == null ? new string[0] : oaisetting.StopSequences.ToArray()) } };
                var url = ServiceUrl.Replace("[$MODEL]", this.Model).Replace("[$API_KEY]", this.ApiKey);
                var res = await Client.PostAsync(url, new StringContent(JsonSerializer.Serialize(json), System.Text.Encoding.UTF8, "application/json"), cancellationToken);
                if (res.IsSuccessStatusCode)
                {
                    var content = await res.Content.ReadAsStringAsync();
                    var obj = JsonSerializer.Deserialize<ResponsePaLMChat>(content);

                    var desc = obj?.candidates?.First().content.parts.First().text;
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
    /*
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
    }*/

    public class ResponsePaLMChat
    {
        public Candidate[] candidates { get; set; }
        public Promptfeedback promptFeedback { get; set; }
    }

    public class Promptfeedback
    {
        public SafetyratingResponse[] safetyRatings { get; set; }
    }

    public class Candidate
    {
        public ContentResponse content { get; set; }
        public string finishReason { get; set; }
        public int index { get; set; }
        public SafetyratingResponse[] safetyRatings { get; set; }
    }

    public class ContentResponse
    {
        public PartResponse[] parts { get; set; }
        public string role { get; set; }
    }

    public class PartResponse
    {
        public string text { get; set; }
    }

    public class SafetyratingResponse
    {
        public string category { get; set; }
        public string probability { get; set; }
    }

    #endregion
    #region models request
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    /*
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
    */

    public class Safetysetting2
    {
        public string category { get; set; }
        public string threshold { get; set; }
    }

    public class RequestPaLMChat
    {
        public Generationconfig generationConfig { get; set; } = new();
        public ContentChat[] contents { get; set; } = new ContentChat[] { new() };
        public Safetysetting2[] safetySettings { get; set; } = GetDefaultSetting();

        static Safetysetting2[] GetDefaultSetting()
        {
            List<Safetysetting2> list = new List<Safetysetting2>();
            list.Add(new Safetysetting2() { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" });
            list.Add(new Safetysetting2() { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_MEDIUM_AND_ABOVE" });
            list.Add(new Safetysetting2() { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_MEDIUM_AND_ABOVE" });
            list.Add(new Safetysetting2() { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" });
            return list.ToArray();
        }
    }

    public class ContentChat
    {
        public string role { get; set; }
        public PartContent[] parts { get; set; } = new PartContent[] { new() };
    }

    public class PartContent
    {
        public string text { get; set; }
        public InlineData? inline_data { get; set; }
    }
    public class InlineData
    {
        public string mime_type { get; set; }
        public string data { get; set; }
    }

    #endregion
}

