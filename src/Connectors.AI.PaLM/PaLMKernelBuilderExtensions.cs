// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Net.Http;
using Connectors.AI.PaLM;
using Connectors.AI.PaLM.TextEmbedding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.PaLM.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.PaLM.TextCompletion;
using Microsoft.SemanticKernel.Connectors.PaLM;
using Microsoft.SemanticKernel.Connectors.PaLM;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.TextGeneration;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace - Using NS of KernelConfig
namespace Microsoft.SemanticKernel;
#pragma warning restore IDE0130

/// <summary>
/// Provides extension methods for the <see cref="KernelBuilder"/> class to configure PaLM connectors.
/// </summary>
public static class PaLMKernelBuilderExtensions
{
    /// <summary>
    /// Adds an PaLM text generation service with the specified configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IKernelBuilder"/> instance to augment.</param>
    /// <param name="model">The name of the PaLM model.</param>
    /// <param name="apiKey">The API key required for accessing the PaLM service.</param>
    /// <param name="endpoint">The endpoint URL for the text generation service.</param>
    /// <param name="serviceId">A local identifier for the given AI service.</param>
    /// <param name="httpClient">The HttpClient to use with this service.</param>
    /// <returns>The same instance as <paramref name="builder"/>.</returns>
    public static IKernelBuilder AddPaLMTextGeneration(
        this IKernelBuilder builder,
        string model,
        string? apiKey = null,
        string? endpoint = null,
        string? serviceId = null,
        HttpClient? httpClient = null)
    {
        VerifyHelper.NotNull(builder);
        VerifyHelper.NotNull(model);

        builder.Services.AddKeyedSingleton<ITextGenerationService>(serviceId, (serviceProvider, _) =>
            new PaLMTextGenerationService(model, apiKey, new HttpClient(), endpoint));

        return builder;
    }

    /// <summary>
    /// Adds an PaLM text generation service with the specified configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to augment.</param>
    /// <param name="model">The name of the PaLM model.</param>
    /// <param name="apiKey">The API key required for accessing the PaLM service.</param>
    /// <param name="endpoint">The endpoint URL for the text generation service.</param>
    /// <param name="serviceId">A local identifier for the given AI service.</param>
    /// <returns>The same instance as <paramref name="services"/>.</returns>
    public static IServiceCollection AddPaLMTextGeneration(
        this IServiceCollection services,
        string model,
        string? apiKey = null,
        string? endpoint = null,
        string? serviceId = null)
    {
        VerifyHelper.NotNull(services);
        VerifyHelper.NotNull(model);

        return services.AddKeyedSingleton<ITextGenerationService>(serviceId, (serviceProvider, _) =>
            new PaLMTextGenerationService(model, apiKey, new HttpClient(), endpoint));
    }


    /// <summary>
    /// Adds an PaLM text embedding generation service with the specified configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IKernelBuilder"/> instance to augment.</param>
    /// <param name="model">The name of the PaLM model.</param>
    /// <param name="endpoint">The endpoint for the text embedding generation service.</param>
    /// <param name="serviceId">A local identifier for the given AI service.</param>
    /// <param name="httpClient">The HttpClient to use with this service.</param>
    /// <returns>The same instance as <paramref name="builder"/>.</returns>
    public static IKernelBuilder AddPaLMTextEmbeddingGeneration(
        this IKernelBuilder builder,
        string model,
        string apiKey,
        string? endpoint = null,
        string? serviceId = null,
        HttpClient? httpClient = null)
    {
        VerifyHelper.NotNull(builder);
        VerifyHelper.NotNull(model);
        VerifyHelper.NotNull(apiKey);

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        builder.Services.AddKeyedSingleton<ITextEmbeddingGenerationService>(serviceId, (serviceProvider, _) =>
            new PaLMTextEmbeddingGenerationService(model, new HttpClient(), endpoint,apiKey));
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        return builder;
    }

    /// <summary>
    /// Adds an PaLM text embedding generation service with the specified configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to augment.</param>
    /// <param name="model">The name of the PaLM model.</param>
    /// <param name="endpoint">The endpoint for the text embedding generation service.</param>
    /// <param name="serviceId">A local identifier for the given AI service.</param>
    /// <returns>The same instance as <paramref name="services"/>.</returns>
    public static IServiceCollection AddPaLMTextEmbeddingGeneration(
        this IServiceCollection services,
        string model,
        string apiKey,
        string? endpoint = null,
        string? serviceId = null)
    {
        VerifyHelper.NotNull(services);
        VerifyHelper.NotNull(model);
        VerifyHelper.NotNull(apiKey);

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return services.AddKeyedSingleton<ITextEmbeddingGenerationService>(serviceId, (serviceProvider, _) =>
            new PaLMTextEmbeddingGenerationService(model, new HttpClient(), endpoint,apiKey));
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }

    

    /// <summary>
    /// Adds the PaLM chat completion service to the list.
    /// </summary>
    /// <param name="builder">The <see cref="IKernelBuilder"/> instance to augment.</param>
    /// <param name="modelId">PaLM model name, see https://platform.PaLM.com/docs/models</param>
    /// <param name="apiKey">PaLM API key, see https://platform.PaLM.com/account/api-keys</param>
    /// <param name="orgId">PaLM organization id. This is usually optional unless your account belongs to multiple organizations.</param>
    /// <param name="serviceId">A local identifier for the given AI service</param>
    /// <param name="httpClient">The HttpClient to use with this service.</param>
    /// <returns>The same instance as <paramref name="builder"/>.</returns>
    public static IKernelBuilder AddPaLMChatCompletion(
        this IKernelBuilder builder,
        string modelId,
        string apiKey,
        string? serviceId = null)
    {
        VerifyHelper.NotNull(builder);
        VerifyHelper.NotNullOrWhiteSpace(modelId);
        VerifyHelper.NotNullOrWhiteSpace(apiKey);

        Func<IServiceProvider, object?, PaLMChatCompletion> factory = (serviceProvider, _) =>
            new(modelId,
                apiKey);

        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);
        builder.Services.AddKeyedSingleton<ITextGenerationService>(serviceId, factory);

        return builder;
    }
}
