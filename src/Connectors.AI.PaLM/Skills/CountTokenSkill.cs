using Microsoft.SemanticKernel;
using System;
//using Microsoft.SemanticKernel.SkillDefinition;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Connectors.AI.PaLM.Skills;

public sealed class TokenSkill:IDisposable
{
    private const string HttpUserAgent = "Microsoft-Semantic-Kernel";
    private readonly string _model;
    private readonly string? _endpoint = "https://generativelanguage.googleapis.com/v1beta2/models";
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;

    /// <summary>
    /// Initializes a new instance of the Token Skill
    /// </summary>
    /// <param name="model">Model to use</param>
    /// <param name="apiKey">PaLM API Key</param>
    /// <param name="httpClient">instance of http client if already exist</param>
    /// <param name="endpoint">PaLM API endpoint</param>
    public TokenSkill(string model, string apiKey, HttpClient? httpClient = null, string? endpoint = null)
    {
        VerifyHelper.NotNullOrWhiteSpace(apiKey);
        VerifyHelper.NotNullOrWhiteSpace(apiKey);

        this._model = model;
        this._apiKey = apiKey;
        this._endpoint = endpoint ?? this._endpoint;
        this._httpClient = httpClient ?? new HttpClient();
    }
    /// <summary>
    /// count tokens from text.
    /// </summary>
    /// <example>
    /// SKContext["input"] = "hello world"
    /// {{token.countToken $input}} => 2
    /// </example>
    /// <param name="input"> The string to count. </param>
    /// <param name="cancellationToken"> cancellation token. </param>
    /// <returns> The token count. </returns>
    [KernelFunction, Description("count token from text.")]
    public async Task<int> CountTokens(string input, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenRequest = new TokenRequest();
            tokenRequest.Prompt.Messages.Add(new MessageToken() { Content = input });

            using var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = this.GetRequestUri(),
                Content = new StringContent(JsonSerializer.Serialize(tokenRequest)),
            };

            httpRequestMessage.Headers.Add("User-Agent", HttpUserAgent);

            var response = await this._httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var tokenResp = JsonSerializer.Deserialize<TokenResponse>(body);

            return tokenResp.TokenCount ?? 0;
        }
        catch (Exception e) 
        {
            throw new Exception(
                
                $"Something went wrong: {e.Message}", e);
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    /// <summary>
    /// Retrieves the request URI based on the provided endpoint and model information.
    /// </summary>
    /// <returns>
    /// A <see cref="Uri"/> object representing the request URI.
    /// </returns>
    private Uri GetRequestUri()
    {
        string? baseUrl = null;

        if (!string.IsNullOrEmpty(this._endpoint))
        {
            baseUrl = this._endpoint;
        }
        else if (this._httpClient.BaseAddress?.AbsoluteUri != null)
        {
            baseUrl = this._httpClient.BaseAddress!.AbsoluteUri;
        }
        else
        {
            throw new Exception( "No endpoint or HTTP client base address has been provided");
        }
        var url = $"{baseUrl!.TrimEnd('/')}/{this._model}:countMessageTokens?key={this._apiKey}";
        return new Uri(url);
    }
}
