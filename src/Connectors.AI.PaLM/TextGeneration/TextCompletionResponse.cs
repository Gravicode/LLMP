// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Connectors.AI.PaLM.TextCompletion;
/*

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
*/

public class TextCompletionResponse
{
    [JsonPropertyName("candidates")]
    public Candidate[] Candidates { get; set; }
    public Promptfeedback promptFeedback { get; set; }
}

public class Promptfeedback
{
    public Safetyrating[] safetyRatings { get; set; }
}

public class Safetyrating
{
    public string category { get; set; }
    public string probability { get; set; }
}

public class Candidate
{
    public Content content { get; set; }
    public string finishReason { get; set; }
    public int index { get; set; }
    public Safetyrating1[] safetyRatings { get; set; }
}

public class Content
{
    public Part[] parts { get; set; }
    public string role { get; set; }
}

public class Part
{
    public string text { get; set; }
}

public class Safetyrating1
{
    public string category { get; set; }
    public string probability { get; set; }
}
