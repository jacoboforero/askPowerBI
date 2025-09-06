using System.Text.Json;
using Azure.AI.OpenAI;
using AskPBI.Api.Models;
using Microsoft.Extensions.Options;

namespace AskPBI.Api.Services;

/// <summary>
/// Azure OpenAI implementation of the intent parsing service
/// </summary>
public class AzureOpenAIIntentService : IIntentService
{
    private readonly OpenAIClient _openAIClient;
    private readonly ILogger<AzureOpenAIIntentService> _logger;
    private readonly AzureOpenAIOptions _options;

    public AzureOpenAIIntentService(
        OpenAIClient openAIClient,
        ILogger<AzureOpenAIIntentService> logger,
        IOptions<AzureOpenAIOptions> options)
    {
        _openAIClient = openAIClient;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<IntentResult> ParseIntentAsync(string query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Parsing intent for query: {Query}", query);

        try
        {
            var prompt = BuildIntentPrompt(query);
            
            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = _options.DeploymentName,
                Messages =
                {
                    new ChatRequestSystemMessage(prompt)
                },
                Temperature = 0.1f, // Low temperature for consistent JSON output
                MaxTokens = 1000
            };

            var response = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions, cancellationToken);
            var content = response.Value.Choices[0].Message.Content;

            _logger.LogDebug("Azure OpenAI response: {Response}", content);

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

    private static string BuildIntentPrompt(string query)
    {
        return $@"You are a business intelligence assistant that parses natural language queries into structured intent.

Parse the following query into a JSON object with this exact structure:
{{
  ""measure"": ""string"",
  ""timeRange"": {{
    ""startDate"": ""YYYY-MM-DD"",
    ""endDate"": ""YYYY-MM-DD"",
    ""description"": ""string""
  }},
  ""grain"": ""string"",
  ""filters"": [
    {{
      ""dimension"": ""string"",
      ""values"": [""string""],
      ""operator"": ""string""
    }}
  ]
}}

VALID MEASURES: {string.Join(", ", ValidMeasures.Measures)}
VALID GRAINS: {string.Join(", ", ValidGrains.Grains)}
VALID DIMENSIONS: {string.Join(", ", ValidDimensions.Dimensions)}

RULES:
1. Extract the main measure/metric from the query
2. Determine the time range (if not specified, use current year)
3. Determine the time grain (if not specified, use ""Month"")
4. Extract any filters (region, product, etc.)
5. Use ""equals"" as the default operator for filters
6. Return ONLY valid JSON, no additional text
7. If a measure is not in the valid list, map it to the closest valid measure
8. If a dimension is not in the valid list, use the exact text from the query

Query to parse: ""{query}""

JSON Response:";
    }

    private static IntentResult ParseAndValidateIntent(string jsonContent, string originalQuery)
    {
        try
        {
            // Clean the JSON content (remove any markdown formatting)
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

            var jsonDocument = JsonDocument.Parse(cleanJson);
            var root = jsonDocument.RootElement;

            // Parse the intent
            var measure = root.GetProperty("measure").GetString() ?? throw new InvalidOperationException("Measure is required");
            var timeRangeElement = root.GetProperty("timeRange");
            var grain = root.GetProperty("grain").GetString() ?? throw new InvalidOperationException("Grain is required");
            var filtersElement = root.GetProperty("filters");

            // Parse time range
            var startDate = DateTime.Parse(timeRangeElement.GetProperty("startDate").GetString()!);
            var endDate = DateTime.Parse(timeRangeElement.GetProperty("endDate").GetString()!);
            var timeDescription = timeRangeElement.GetProperty("description").GetString() ?? "";

            var timeRange = new TimeRange
            {
                StartDate = startDate,
                EndDate = endDate,
                Description = timeDescription
            };

            // Parse filters
            var filters = new List<Filter>();
            foreach (var filterElement in filtersElement.EnumerateArray())
            {
                var dimension = filterElement.GetProperty("dimension").GetString() ?? "";
                var operatorValue = filterElement.GetProperty("operator").GetString() ?? "equals";
                var values = new List<string>();
                
                foreach (var valueElement in filterElement.GetProperty("values").EnumerateArray())
                {
                    values.Add(valueElement.GetString() ?? "");
                }

                if (!string.IsNullOrEmpty(dimension) && values.Count > 0)
                {
                    filters.Add(new Filter
                    {
                        Dimension = dimension,
                        Values = values,
                        Operator = operatorValue
                    });
                }
            }

            // Validate the parsed intent
            ValidateIntent(measure, grain, filters);

            return new IntentResult
            {
                Measure = measure,
                TimeRange = timeRange,
                Grain = grain,
                Filters = filters,
                OriginalQuery = originalQuery
            };
        }
        catch (JsonException ex)
        {
            throw new IntentParsingException($"Invalid JSON response from Azure OpenAI: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new IntentParsingException($"Failed to parse intent from JSON: {ex.Message}", ex);
        }
    }

    private static void ValidateIntent(string measure, string grain, List<Filter> filters)
    {
        var validationErrors = new List<string>();

        // Validate measure
        if (!ValidMeasures.Measures.Contains(measure))
        {
            validationErrors.Add($"Invalid measure: {measure}. Valid measures are: {string.Join(", ", ValidMeasures.Measures)}");
        }

        // Validate grain
        if (!ValidGrains.Grains.Contains(grain))
        {
            validationErrors.Add($"Invalid grain: {grain}. Valid grains are: {string.Join(", ", ValidGrains.Grains)}");
        }

        // Validate filter dimensions
        foreach (var filter in filters)
        {
            if (!ValidDimensions.Dimensions.Contains(filter.Dimension))
            {
                validationErrors.Add($"Invalid filter dimension: {filter.Dimension}. Valid dimensions are: {string.Join(", ", ValidDimensions.Dimensions)}");
            }
        }

        if (validationErrors.Count > 0)
        {
            throw new IntentValidationException($"Intent validation failed: {string.Join("; ", validationErrors)}");
        }
    }
}

/// <summary>
/// Configuration options for Azure OpenAI
/// </summary>
public class AzureOpenAIOptions
{
    public const string SectionName = "AzureOpenAI";
    
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = string.Empty;
}

/// <summary>
/// Exception thrown when intent parsing fails
/// </summary>
public class IntentParsingException : Exception
{
    public IntentParsingException(string message) : base(message) { }
    public IntentParsingException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when intent validation fails
/// </summary>
public class IntentValidationException : Exception
{
    public IntentValidationException(string message) : base(message) { }
}
