// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.Connectors.AI.PaLM.ChatCompletion;

/// <summary>
/// OpenAI Chat content
/// See https://platform.openai.com/docs/guides/chat for details
/// </summary>
public class PaLMChatHistory : ChatHistory
{
    /// <summary>
    /// Create a new and empty chat history
    /// </summary>
    /// <param name="assistantInstructions">Optional instructions for the assistant</param>
    public PaLMChatHistory(string? assistantInstructions = null)
    {
        if (!string.IsNullOrWhiteSpace( assistantInstructions))
        {
            this.AddSystemMessage(assistantInstructions);
        }
    }

    public static PaLMChatHistory FromChatHistory(ChatHistory history)
    {
        var temp = new PaLMChatHistory();
        foreach(ChatMessageContent item in history)
        {
            temp.Add(item);
        }
        return temp;
    }
}
