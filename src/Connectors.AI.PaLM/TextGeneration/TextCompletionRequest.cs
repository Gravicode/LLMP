// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.AI.PaLM.TextCompletion;

/// <summary>
/// HTTP schema to perform completion request.
/// </summary>
[Serializable]
public sealed class TextCompletionRequest
{
    ///// <summary>
    ///// Prompt to complete.
    ///// </summary>
    [JsonPropertyName("prompt")]
    public Prompt Prompt { get; set; } = new();
    [JsonPropertyName("temperature")]
    public float Temperature { get; set; } = 0.1f;
    [JsonPropertyName("top_p")]
    public float TopP { get; set; } = 0.95f;
    [JsonPropertyName("candidate_count")]
    public int CandidateCount { get; set; } = 3;
    [JsonPropertyName("top_k")]
    public int TopK { get; set; } = 40;
    [JsonPropertyName("max_output_tokens")]
    public int MaxOutputTokens { get; set; } = 2048;
    [JsonPropertyName("stop_sequences")]
    public string[] StopSequences { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Text prompt
/// </summary>
[Serializable]
public class Prompt
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
