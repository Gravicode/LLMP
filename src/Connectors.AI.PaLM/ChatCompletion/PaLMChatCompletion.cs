// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Connectors.AI.PaLM;
using Connectors.AI.PaLM.Helper;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextGeneration;

namespace Microsoft.SemanticKernel.Connectors.AI.PaLM.ChatCompletion;

/// <summary>
/// PaLM chat completion client.
/// TODO: forward ETW logging to ILogger, see https://learn.microsoft.com/en-us/dotnet/azure/sdk/logging
/// </summary>
public sealed class PaLMChatCompletion : IChatCompletionService, ITextGenerationService //:IAIService
{
    PaLMClient client { get; set; }

    public IReadOnlyDictionary<string, string> Attributes => _attributes;

    IReadOnlyDictionary<string, object> IAIService.Attributes => _objattributes;

    private readonly Dictionary<string, string> _attributes = new();
    private readonly Dictionary<string, object> _objattributes = new();
    /// <summary>
    /// Create an instance of the PaLM chat completion connector
    /// </summary>
    /// <param name="modelId">Model name</param>
    /// <param name="apiKey">PaLM API Key</param>

    public PaLMChatCompletion(
        string modelId,
        string apiKey
       )
    {
        VerifyHelper.NotNullOrWhiteSpace(modelId);
        VerifyHelper.NotNullOrWhiteSpace(apiKey);
        this._attributes.Add(AIServiceExtensions.ModelIdKey, modelId);
       
        this.client = new PaLMClient(apiKey,modelId);
    }

    /// <inheritdoc/>
    public ChatHistory CreateNewChat(string? instructions = null)
    {
        return new PaLMChatHistory(instructions);
    }

    public async Task<IReadOnlyList<ChatMessageContent>> GenerateMessageAsync(PaLMChatHistory chat, PromptExecutionSettings requestSettings = null, CancellationToken cancellationToken = default)
    {
        var data = await this.client.GetMessageAsync(chat, requestSettings, cancellationToken);
        var content = new ChatMessageContent(AuthorRole.Assistant, content:data, modelId:this.GetModelId());
        var readOnlyList = new ReadOnlyCollection<ChatMessageContent>(new List<ChatMessageContent> { content });
        return readOnlyList;
    }
    
    public async Task<IReadOnlyList<TextContent>> GenerateMessageAsync(string prompt, PromptExecutionSettings requestSettings = null, CancellationToken cancellationToken = default)
    {
        var chat = new PaLMChatHistory();
        chat.AddUserMessage(prompt);
        var data = await this.client.GetMessageAsync(chat, requestSettings, cancellationToken);
        var content = new TextContent(data, modelId:this.GetModelId());
        var readOnlyList = new ReadOnlyCollection<TextContent>(new List<TextContent> { content });
        return readOnlyList;
    }


    /// <inheritdoc/>
    public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        => this.GenerateMessageAsync(PaLMChatHistory.FromChatHistory(chatHistory), executionSettings, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        => throw new System.NotImplementedException();

    /// <inheritdoc/>
    public Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        => this.GenerateMessageAsync(prompt, executionSettings, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        => throw new System.NotImplementedException();
}
