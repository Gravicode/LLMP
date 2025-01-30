using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Web.Google;
using LLMP.Desktop.Data;



namespace LLMP.Desktop.Services
{

    public class SearchService
    {
        public OpenAIPromptExecutionSettings Setting { set; get; }
        public bool IsReady { get; set; }
        public string SkillName { get; set; } = "SearchSkill";
        public string FunctionName { set; get; } = "Search";
     
        public bool IsProcessing { get; set; } = false;
        public string SelectedModel { get; set; }
        Dictionary<string, KernelFunction> ListFunctions = new Dictionary<string, KernelFunction>();

        Kernel kernel { set; get; }

        public SearchService()
        {
            Setting = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 2000,
                Temperature = 0.7,
                TopP = 0.5
            };
        }
        public void Setup(string SelectedModel)
        {
            this.SelectedModel = SelectedModel;
            if (SelectedModel.Contains("bison") || SelectedModel.Contains("gecko") || SelectedModel.Contains("gemini") || SelectedModel.Contains("embedding"))
            {
                //palm
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                kernel = Kernel.CreateBuilder()
            .AddGoogleAIGeminiChatCompletion(modelId: SelectedModel, apiKey: AppConstants.PalmKey, serviceId: "davinci")
       .Build();
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            }
            else if (SelectedModel.Contains("deepseek", StringComparison.InvariantCultureIgnoreCase))
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
            .AddOpenAIChatCompletion(modelId: SelectedModel, apiKey: AppConstants.OpenAIKey, orgId: AppConstants.OpenAIOrg, serviceId: "davinci")
       .Build();

            }

            // Load Bing skill
#pragma warning disable SKEXP0054 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            //var bingConnector = new BingConnector(AppConstants.BingKey);
            var googleConnector = new GoogleConnector(AppConstants.GoogleSearchKey, AppConstants.GoogleSearchID);
            kernel.ImportPluginFromObject(new WebSearchEnginePlugin(googleConnector), "bing");
#pragma warning restore SKEXP0054 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            SetupSkill();
            IsReady = true;
        }
        public void SetupSkill()
        {
          
          
            var skPrompt = @"Answer questions only when you know the facts or the information is provided.
When you don't have sufficient information you reply with a list of commands to find the information needed.
When answering multiple questions, use a bullet point list.
Note: make sure single and double quotes are escaped using a backslash char.

[COMMANDS AVAILABLE]
- bing.search

[INFORMATION PROVIDED]
{{ $externalInformation }}

[EXAMPLE 1]
Question: what's the biggest lake in Italy?
Answer: Lake Garda, also known as Lago di Garda.

[EXAMPLE 2]
Question: what's the biggest lake in Italy? What's the smallest positive number?
Answer:
* Lake Garda, also known as Lago di Garda.
* The smallest positive number is 1.

[EXAMPLE 3]
Question: what's Ferrari stock price? Who is the current number one female tennis player in the world?
Answer:
{{ '{{' }} bing.search ""what\\'s Ferrari stock price?"" {{ '}}' }}.
{{ '{{' }} bing.search ""Who is the current number one female tennis player in the world?"" {{ '}}' }}.

[END OF EXAMPLES]

[TASK]
Question: {{ $question }}.
Answer: ";

            
            var SearchFunction = kernel.CreateFunctionFromPrompt(skPrompt, executionSettings: Setting, functionName: FunctionName);

            ListFunctions.Add(FunctionName, SearchFunction);
        }

        public KernelFunction GetFunction()
        {
            return ListFunctions.Values.FirstOrDefault();
        }

        public async Task<string> Search(string query)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
              
                IsProcessing = true;
                var context = new KernelArguments();
                context.Add("externalInformation", "");
                context.Add("question", query);

                var function = ListFunctions[FunctionName];
                var answer = await function.InvokeAsync(kernel, context);
                // If the answer contains commands, execute them using the prompt renderer.
                var res = answer.GetValue<string>();
                if (res.Contains("bing.search", StringComparison.OrdinalIgnoreCase))
                {
                    var promptTemplateFactory = new KernelPromptTemplateFactory();
                    var promptTemplate = promptTemplateFactory.Create(new PromptTemplateConfig(answer.GetValue<string>()));

                    Console.WriteLine("---- Fetching information from Bing...");
                    var information = await promptTemplate.RenderAsync(kernel);

                    Console.WriteLine("Information found:");
                    Console.WriteLine(information);

                    // The rendered prompt contains the information retrieved from search engines
                    context["externalInformation"] = information;
                    context["question"] = query;

                    // Run the semantic function again, now including information from Bing
                    answer = await function.InvokeAsync(kernel, context);
                }
                else
                {
                    Console.WriteLine("AI had all the information, no need to query Bing.");
                }
                Console.WriteLine(answer);
                Result = answer.GetValue<string>().Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return ex.ToString();
            }
            finally
            {
                IsProcessing = false;
            }
            if (Result.Contains("bing.search"))
            {
                Result = "coba beberapa saat lagi, search api ada limit request per menit.";
            }
            return Result;
        }

    }
}
