// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Connectors.AI.PaLM.Skills;
/*
/// <summary>
/// text 
/// </summary>
public class MessageToken
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
/// <summary>
/// contain collection of text for token counting
/// </summary>
public class PromptToken
{
    [JsonPropertyName("messages")]
    public List<MessageToken> Messages { get; set; } = new();
}
/// <summary>
/// class for request token count
/// </summary>
public class TokenRequest
{
    [JsonPropertyName("prompt")]
    public PromptToken Prompt { get; set; } = new();
}
*/

public class TokenRequest
{
    public Content[] contents { get; set; } = new Content[] { new Content () };
}

public class Content
{
    public Part[] parts { get; set; } = new Part[] { new() };
}

public class Part
{
    public string text { get; set; }
}
