using LLMP.Desktop.Data;
using LLMP.Models;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace LLMP.Desktop.Services
{
    public class ImageStyles
    {
        public const string Vivid = "vivid";
        public const string Natural = "natural";
    }
    public class ImageGeneratorService
    {
        public string Username { get; set; } = "TestUser";
        IOpenAIService openAiService { set; get; }
        public bool IsProcessing { get; set; } = false;
        /// <summary>
        /// Create a new instance of OpenAI image generation service
        /// </summary>
        /// <param name="apiKey">OpenAI API key, see https://platform.openai.com/account/api-keys</param>
        /// <param name="organization">OpenAI organization id. This is usually optional unless your account belongs to multiple organizations.</param>
        /// <param name="handlerFactory">Retry handler</param>
        /// <param name="log">Logger</param>
        public ImageGeneratorService(IOpenAIService service)
        {
            this.openAiService = service;
        }

        public async Task<(string Message, List<ImageModel> Result)> GenerateImageAsync(string Prompt, int NumImages, string ImageSize, string ModelId, string Style = ImageStyles.Natural, CancellationToken cancellationToken = default)
        {
            if (IsProcessing) return default;
            try
            {
                IsProcessing = true;
                var results = new List<ImageModel>();
                var ErrorMsg = string.Empty;

                //for (var i = 0; i < NumImages; i++)
                //{
                var imageResult = await openAiService.Image.CreateImage(new ImageCreateRequest
                {
                    Prompt = Prompt,
                    N = NumImages,
                    Size = ImageSize,
                    ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                    User = Username,
                    Model = ModelId,
                    Style = Style
                });


                if (imageResult.Successful)
                {

                    Console.WriteLine(string.Join("\n", imageResult.Results.Select(r => r.Url)));
                    var count = 1;
                    results.AddRange(imageResult.Results.Select(x => new ImageModel() { Url = x.Url, Title = $"Image-{count++}" }));
                    return (null, results);
                }
                else
                {
                    var res = imageResult.Error.Message;
                    Console.WriteLine(res);
                    ErrorMsg = res;
                    return (res, null);
                    //results.Add(res);
                }
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Snackbar.Add($"failed to generate image: {ex}", Severity.Error);
            }
            finally
            {
                IsProcessing = false;
            }
            return default;
        }

        public async Task<List<ImageModel>> GenerateImageEditAsync(byte[] ImageData, string ImageFileName, string Prompt, byte[] MaskData, string MaskFileName, int NumberImages, string ImageSize, CancellationToken cancellationToken = default)
        {
            if (IsProcessing) return default;
            try
            {
                IsProcessing = true;
                var results = new List<ImageModel>();
                var ErrorMsg = string.Empty;
                var imageResult = await openAiService.Image.CreateImageEdit(new ImageEditCreateRequest
                {
                    Image = ImageData,
                    ImageName = ImageFileName,
                    Prompt = Prompt,
                    Mask = MaskData,
                    MaskName = MaskFileName,
                    N = NumberImages,
                    Size = ImageSize,
                    ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                    User = Username,
                    //Model = AppConstants.ImageModel3
                });


                if (imageResult.Successful)
                {

                    Console.WriteLine(string.Join("\n", imageResult.Results.Select(r => r.Url)));
                    var count = 1;
                    results.AddRange(imageResult.Results.Select(x => new ImageModel() { Url = x.Url, Title = $"Image-{count++}" }));
                }
                else
                {
                    var res = imageResult.Error.Message;
                    Console.WriteLine(res);
                    ErrorMsg = res;
                    //results.Add(res);
                }
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Snackbar.Add($"failed to generate image: {ex}", Severity.Error);
            }
            finally
            {
                IsProcessing = false;
            }
            return default;
        }

        public async Task<List<ImageModel>> GenerateImageVariationsAsync(byte[] ImageData, string ImageFileName, int NumberImages, string ImageSize, CancellationToken cancellationToken = default)
        {
            if (IsProcessing) return default;
            try
            {
                IsProcessing = true;
                var results = new List<ImageModel>();
                var ErrorMsg = string.Empty;
                var imageResult = await openAiService.Image.CreateImageVariation(new ImageVariationCreateRequest
                {
                    Image = ImageData,
                    ImageName = ImageFileName,
                    N = NumberImages,
                    Size = ImageSize,
                    ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                    User = Username,
                    Model = AppConstants.ImageModel2
                });


                if (imageResult.Successful)
                {

                    Console.WriteLine(string.Join("\n", imageResult.Results.Select(r => r.Url)));
                    var count = 1;
                    results.AddRange(imageResult.Results.Select(x => new ImageModel() { Url = x.Url, Title = $"Image-{count++}" }));
                }
                else
                {
                    var res = imageResult.Error.Message;
                    Console.WriteLine(res);
                    ErrorMsg = res;
                    //results.Add(res);
                }
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Snackbar.Add($"failed to generate image: {ex}", Severity.Error);
            }
            finally
            {
                IsProcessing = false;
            }
            return default;
        }
    }
    /*
    public class ImageGeneratorService
    {
        public Size size { set; get; } = new Size(1024,1024);
        Kernel kernel { set; get; }
        public ImageGeneratorService()
        {
            Setup();
        }

        public async Task Setup()
        {
            // Configure the two AI features: OpenAI Chat and DALL-E 3 for image generation
            var builder = Kernel.CreateBuilder();

           
                builder.AddOpenAIChatCompletion(AppConstants.DefaultModel, AppConstants.OpenAIKey, AppConstants.OpenAIOrg);
#pragma warning disable SKEXP0012 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            builder.AddOpenAITextToImage(AppConstants.OpenAIKey, AppConstants.OpenAIOrg);
#pragma warning restore SKEXP0012 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


            kernel = builder.Build();

         

            // Get AI service instance used to manage the user chat
            //var chatGPT = kernel.GetRequiredService<IChatCompletionService>();
        }

        public async Task<string> GenerateImage(string Message)
        {   // Get AI service instance used to generate images
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable SKEXP0002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var dallE = kernel.GetRequiredService<ITextToImageService>();
#pragma warning restore SKEXP0002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable SKEXP0002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var imageUrl = await dallE.GenerateImageAsync(Message, size.Width, size.Height);
#pragma warning restore SKEXP0002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            return imageUrl;
        }
    }
    */
}
