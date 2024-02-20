// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.AI.PaLM.TextCompletion;
/*
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
*/

[Serializable]
public sealed class TextCompletionRequest
{
    public Content[] contents { get; set; } = new Content[] { new Content() };
    public Generationconfig generationConfig { get; set; } = new();
    public Safetysetting[] safetySettings { get; set; } = GetDefaultSetting();

    static Safetysetting[] GetDefaultSetting()
    {
        List<Safetysetting> list = new List<Safetysetting>();
        list.Add(new Safetysetting() { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" });
        list.Add(new Safetysetting() { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_MEDIUM_AND_ABOVE" });
        list.Add(new Safetysetting() { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_MEDIUM_AND_ABOVE" });
        list.Add(new Safetysetting() { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" });
        return list.ToArray();
    }
}

public class Generationconfig
{
    public float temperature { get; set; } = 1.0f;
    public float topK { get; set; } = 1.0f;
    public float topP { get; set; } = 1.0f;
    public int maxOutputTokens { get; set; } = 2048;
    public object[] stopSequences { get; set; } = new object[] { };
}

public class Content
{
    public Part[] parts { get; set; } = new Part[] { new Part() };
}

public class Part
{
    public string text { get; set; }
}

public class Safetysetting
{
    public string category { get; set; }
    public string threshold { get; set; }
}


