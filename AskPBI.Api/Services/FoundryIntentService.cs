using System.Text.Json;
using AskPBI.Api.Models;

namespace AskPBI.Api.Services;

/// <summary>
/// Intent parsing service using Azure AI Foundry
/// </summary>
public class FoundryIntentService : IIntentService
{
    private readonly FoundryClient _foundryClient;
    private readonly ILogger<FoundryIntentService> _logger;

    public FoundryIntentService(FoundryClient foundryClient, ILogger<FoundryIntentService> logger)
    {
        _foundryClient = foundryClient;
        _logger = logger;
    }

    public async Task<IntentResult> ParseIntentAsync(string query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Parsing intent for query: {Query}", query);

        try
        {
            // Build intent-specific prompt
            var systemPrompt = BuildIntentPrompt();
            
            // Call Foundry API with intent-specific prompt
            var foundryResponse = await _foundryClient.SendMessageAsync(systemPrompt, query, cancellationToken);
            var content = foundryResponse.Choices.First().Message.Content;

            _logger.LogDebug("Foundry response content: {Content}", content);

            // Parse the JSON response
            var intentResult = ParseAndValidateIntent(content, query);
            
            _logger.LogInformation("Successfully parsed intent: {Intent}", JsonSerializer.Serialize(intentResult));
            
            return intentResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse intent for query: {Query}", query);
            throw new IntentParsingException($"Failed to parse intent: {ex.Message}", ex);
        }
    }

    private static IntentResult ParseAndValidateIntent(string jsonContent, string originalQuery)
    {
        try
        {
            // Clean up the JSON content (remove any markdown formatting)
            var cleanJson = jsonContent.Trim();
            if (cleanJson.StartsWith("```json"))
            {
                cleanJson = cleanJson.Substring(7);
            }
            if (cleanJson.EndsWith("```"))
            {
                cleanJson = cleanJson.Substring(0, cleanJson.Length - 3);
            }
            cleanJson = cleanJson.Trim();

            // Parse JSON
            var intentData = JsonSerializer.Deserialize<JsonElement>(cleanJson);
            
            // Extract and validate required fields
            var measure = GetRequiredString(intentData, "measure");
            var grain = GetRequiredString(intentData, "grain");
            
            // Parse time range
            var timeRangeElement = intentData.GetProperty("timeRange");
            var timeRange = new TimeRange
            {
                StartDate = GetRequiredString(timeRangeElement, "startDate"),
                EndDate = GetRequiredString(timeRangeElement, "endDate"),
                Description = GetRequiredString(timeRangeElement, "description")
            };

            // Parse filters
            var filters = new List<Filter>();
            if (intentData.TryGetProperty("filters", out var filtersElement) && filtersElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var filterElement in filtersElement.EnumerateArray())
                {
                    filters.Add(new Filter
                    {
                        Dimension = GetRequiredString(filterElement, "dimension"),
                        Value = GetRequiredString(filterElement, "value")
                    });
                }
            }

            return new IntentResult
            {
                Measure = measure,
                TimeRange = timeRange,
                Grain = grain,
                Filters = filters,
                OriginalQuery = originalQuery
            };
        }
        catch (Exception ex)
        {
            throw new IntentParsingException($"Failed to parse JSON response: {ex.Message}", ex);
        }
    }

    private static string GetRequiredString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.String)
        {
            throw new IntentParsingException($"Missing or invalid required property: {propertyName}");
        }
        return property.GetString() ?? throw new IntentParsingException($"Property {propertyName} is null");
    }

    private static string BuildIntentPrompt()
    {
        return @"You are a business intelligence assistant that parses natural language queries into structured intent.

Parse the following query into a JSON object with this exact structure:
{
  ""measure"": ""string"",
  ""timeRange"": {
    ""startDate"": ""YYYY-MM-DD"",
    ""endDate"": ""YYYY-MM-DD"",
    ""description"": ""string""
  },
  ""grain"": ""string"",
  ""filters"": [
    {
      ""dimension"": ""string"",
      ""value"": ""string""
    }
  ]
}

Rules:
- measure: The business metric (Revenue, Sales, Profit, Customer Count, etc.)
- timeRange: Extract start/end dates and human description
- grain: Time granularity (Day, Week, Month, Quarter, Year)
- filters: Any dimensional filters (Region, Product, Category, etc.)
- Return ONLY valid JSON, no other text
- Use current date as reference for relative dates
- If no specific time mentioned, default to last 12 months

Examples:
Query: ""Revenue by month in 2024""
Response: {""measure"":""Revenue"",""timeRange"":{""startDate"":""2024-01-01"",""endDate"":""2024-12-31"",""description"":""2024""},""grain"":""Month"",""filters"":[]}

Query: ""Sales in North America for Q1""
Response: {""measure"":""Sales"",""timeRange"":{""startDate"":""2024-01-01"",""endDate"":""2024-03-31"",""description"":""Q1 2024""},""grain"":""Quarter"",""filters"":[{""dimension"":""Region"",""value"":""North America""}]}";
    }
}
