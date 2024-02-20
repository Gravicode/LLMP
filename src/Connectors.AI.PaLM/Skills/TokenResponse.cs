// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Connectors.AI.PaLM.Skills;
public class TokenResponse
{
    [JsonPropertyName("tokenCount")]
    public int? TokenCount { get; set; }
}
