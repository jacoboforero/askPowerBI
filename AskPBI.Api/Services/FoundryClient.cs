using Azure;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Chat;
using OpenAI.Chat;
using Microsoft.Extensions.Options;

namespace AskPBI.Api.Services;

/// <summary>
/// Client for calling Azure AI Foundry deployments using the official SDK
/// </summary>
public class FoundryClient
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<FoundryClient> _logger;
    private readonly FoundryOptions _options;

    public FoundryClient(ILogger<FoundryClient> logger, IOptions<FoundryOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        
        // Create Azure OpenAI client using the official SDK
        var endpoint = new Uri(_options.Endpoint);
        var azureClient = new AzureOpenAIClient(endpoint, new AzureKeyCredential(_options.ApiKey));
        _chatClient = azureClient.GetChatClient(_options.DeploymentName);
    }

    /// <summary>
    /// Sends a message to the Foundry deployment and returns the response
    /// </summary>
    /// <param name="systemPrompt">The system prompt to set context</param>
    /// <param name="userMessage">The user message to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The AI response</returns>
    public async Task<FoundryResponse> SendMessageAsync(string systemPrompt, string userMessage, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending message to Foundry deployment");

        try
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userMessage)
            };

            // Use the simplest possible call for gpt-5-mini
            var response = await _chatClient.CompleteChatAsync(messages);
            
            _logger.LogDebug("Foundry API response: {Response}", response.Value.Content[0].Text);

            var foundryResponse = new FoundryResponse
            {
                Choices = new List<FoundryChoice>
                {
                    new FoundryChoice
                    {
                        Message = new FoundryMessage
                        {
                            Content = response.Value.Content[0].Text
                        }
                    }
                }
            };

            return foundryResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call Foundry API");
            throw;
        }
    }

}

/// <summary>
/// Configuration options for Foundry client
/// </summary>
public class FoundryOptions
{
    public const string SectionName = "Foundry";
    
    public required string Endpoint { get; set; }
    public required string ApiKey { get; set; }
    public required string DeploymentName { get; set; }
}

/// <summary>
/// Response from Foundry API
/// </summary>
public class FoundryResponse
{
    public List<FoundryChoice> Choices { get; set; } = new();
}

public class FoundryChoice
{
    public FoundryMessage Message { get; set; } = new();
}

public class FoundryMessage
{
    public string Content { get; set; } = string.Empty;
}
