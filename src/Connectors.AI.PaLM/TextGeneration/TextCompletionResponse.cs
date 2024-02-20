// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Connectors.AI.PaLM.TextCompletion;

public class TextCompletionResponse
{
    [JsonPropertyName("candidates")]
    public Candidate[]? Candidates { get; set; }
}

public class Candidate
{
    [JsonPropertyName("output")]
    public string? Output { get; set; }
    [JsonPropertyName("safetyRatings")]
    public Safetyrating[]? SafetyRatings { get; set; }
}

public class Safetyrating
{
    [JsonPropertyName("category")]
    public string? Category { get; set; }
    [JsonPropertyName("probability")]
    public string? Probability { get; set; }
}
