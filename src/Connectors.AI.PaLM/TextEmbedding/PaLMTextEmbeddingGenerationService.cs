// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Connectors.AI.PaLM;
using Connectors.AI.PaLM.Helper;
using Connectors.AI.PaLM.TextEmbedding;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.PaLM.TextEmbedding;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Http;
using Microsoft.SemanticKernel.Services;

namespace Connectors.AI.PaLM;

/// <summary>
/// PaLM embedding generation service.
/// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable. No need to dispose the Http client here. It can either be an internal client using NonDisposableHttpClientHandler or an external client managed by the calling code, which should handle its disposal.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
public sealed class PaLMTextEmbeddingGenerationService : ITextEmbeddingGenerationService
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore CA1001 // Types that own disposable fields should be disposable. No need to dispose the Http client here. It can either be an internal client using NonDisposableHttpClientHandler or an external client managed by the calling code, which should handle its disposal.
{
    private readonly string? _apiKey;
    private const string HttpUserAgent = "Microsoft-Semantic-Kernel";
    //private readonly string _model = "embedding-gecko-001";
    private readonly string _model = "embedding-001";
    private readonly string? _endpoint;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, object?> _attributes = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PaLMTextEmbeddingGenerationService"/> class.
    /// Using default <see cref="HttpClientHandler"/> implementation.
    /// </summary>
    /// <param name="endpoint">Endpoint for service API call.</param>
    /// <param name="model">Model to use for service API call.</param>
    public PaLMTextEmbeddingGenerationService(Uri endpoint, string model)
    {
        VerifyHelper.NotNull(endpoint);
        VerifyHelper.NotNullOrWhiteSpace(model);

        this._model = model;
        this._endpoint = endpoint.AbsoluteUri;
        this._attributes.Add(AIServiceExtensions.ModelIdKey, this._model);
        this._attributes.Add(AIServiceExtensions.EndpointKey, this._endpoint);
        this._httpClient = new HttpClient();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaLMTextEmbeddingGenerationService"/> class.
    /// </summary>
    /// <param name="model">Model to use for service API call.</param>
    /// <param name="endpoint">Endpoint for service API call.</param>
    public PaLMTextEmbeddingGenerationService(string model, string endpoint, string? apiKey = null)
    {
        VerifyHelper.NotNullOrWhiteSpace(model);
        VerifyHelper.NotNullOrWhiteSpace(endpoint);

        this._model = model;
        this._endpoint = endpoint;
        this._apiKey = apiKey;
        this._attributes.Add(AIServiceExtensions.ModelIdKey, this._model);
        this._attributes.Add(AIServiceExtensions.EndpointKey, this._endpoint);
        this._httpClient = new HttpClient();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaLMTextEmbeddingGenerationService"/> class.
    /// </summary>
    /// <param name="model">Model to use for service API call.</param>
    /// <param name="httpClient">The HttpClient used for making HTTP requests.</param>
    /// <param name="endpoint">Endpoint for service API call. If not specified, the base address of the HTTP client is used.</param>
    public PaLMTextEmbeddingGenerationService(string model, HttpClient httpClient, string? endpoint = null, string? apiKey = null)
    {
        VerifyHelper.NotNullOrWhiteSpace(model);
        VerifyHelper.NotNull(httpClient);
        if (httpClient.BaseAddress == null && string.IsNullOrEmpty(endpoint))
        {
            throw new ArgumentException($"The {nameof(httpClient)}.{nameof(HttpClient.BaseAddress)} and {nameof(endpoint)} are both null or empty. Please ensure at least one is provided.");
        }

        this._model = model;
        this._endpoint = endpoint;
        this._httpClient = httpClient;
        this._apiKey = apiKey;
        this._attributes.Add(AIServiceExtensions.ModelIdKey, model);
        this._attributes.Add(AIServiceExtensions.EndpointKey, endpoint ?? httpClient.BaseAddress!.ToString());
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, object?> Attributes => this._attributes;

    /// <inheritdoc/>
    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(
        IList<string> data,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        return await this.ExecuteEmbeddingRequestAsync(data, cancellationToken).ConfigureAwait(false);
    }

    #region private ================================================================================

    /// <summary>
    /// Performs HTTP request to given endpoint for embedding generation.
    /// </summary>
    /// <param name="data">Data to embed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>List of generated embeddings.</returns>
    private async Task<IList<ReadOnlyMemory<float>>> ExecuteEmbeddingRequestAsync(IList<string> data, CancellationToken cancellationToken)
    {
        var embeddingRequest = new TextEmbeddingRequest
        { 
            //Text = string.Join(" ", data)
        };
        var items = new List<Part>();
        foreach(var item in data)
        {
            items.Add(new Part() { text = item });
        }
        embeddingRequest.content.parts = items.ToArray();
        using var httpRequestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = this.GetRequestUri(),
            Content = new StringContent(JsonSerializer.Serialize(embeddingRequest)),
        };

        httpRequestMessage.Headers.Add("User-Agent", HttpUserAgent);

        var response = await this._httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var embeddingResponse = JsonSerializer.Deserialize<TextEmbeddingResponse>(body);
        if (embeddingResponse is null || embeddingResponse.embedding is null)
        {
            var errorCls = JsonSerializer.Deserialize<ErrorPalm>(body);
            if (errorCls != null)
            {
                throw new KernelException(
                    $"{errorCls.error.code}-{errorCls.error.status}: {errorCls.error.message}");
            }
        }
        return new List<ReadOnlyMemory<float>>() { new ReadOnlyMemory<float>(embeddingResponse?.embedding.values) };

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
            throw new KernelException("No endpoint or HTTP client base address has been provided");
        }

        //var url = $"{baseUrl!.TrimEnd('/')}/{this._model}:embedText?key={this._apiKey}";
        var url = $"{baseUrl!.TrimEnd('/')}/{this._model}:embedContent?key={this._apiKey}";

        return new Uri(url);
    }

    #endregion
}
