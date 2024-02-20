// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;

namespace Connectors.AI.PaLM.TextEmbedding;

/// <summary>
/// embedding values in float 
/// </summary>
public class Embedding
{
    public List<float>? value { get; set; }
}

/// <summary>
/// response from embedding function
/// </summary>
public sealed class TextEmbeddingResponse
{
    public Embedding? embedding { get; set; }
}
