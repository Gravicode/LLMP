// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.AI.PaLM.TextEmbedding;
/*
/// <summary>
/// HTTP schema to perform embedding request.
/// </summary>
[Serializable]
public sealed class TextEmbeddingRequest
{
    /// <summary>
    /// Data to embed.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
*/

public class TextEmbeddingRequest
{
    public string model { get; set; }
    public Content content { get; set; } = new();
}

public class Content
{
    public Part[] parts { get; set; } = new Part[] { new Part () };
}

public class Part
{
    public string text { get; set; }
}
