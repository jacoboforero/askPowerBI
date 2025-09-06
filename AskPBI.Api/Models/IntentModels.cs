namespace AskPBI.Api.Models;

/// <summary>
/// Represents the structured intent extracted from a natural language query
/// </summary>
public record IntentResult
{
    /// <summary>
    /// The measure/metric being requested (e.g., "Revenue", "Sales", "Profit")
    /// </summary>
    public required string Measure { get; init; }
    
    /// <summary>
    /// The time range for the analysis
    /// </summary>
    public required TimeRange TimeRange { get; init; }
    
    /// <summary>
    /// The granularity of the time dimension (e.g., "Month", "Quarter", "Year")
    /// </summary>
    public required string Grain { get; init; }
    
    /// <summary>
    /// Any filters applied to the data (e.g., region, product category)
    /// </summary>
    public List<Filter> Filters { get; init; } = new();
    
    /// <summary>
    /// The original query that was parsed
    /// </summary>
    public required string OriginalQuery { get; init; }
}

/// <summary>
/// Represents a time range for analysis
/// </summary>
public record TimeRange
{
    /// <summary>
    /// Start date of the time range
    /// </summary>
    public required string StartDate { get; init; }
    
    /// <summary>
    /// End date of the time range
    /// </summary>
    public required string EndDate { get; init; }
    
    /// <summary>
    /// Human-readable description of the time range
    /// </summary>
    public required string Description { get; init; }
}

/// <summary>
/// Represents a filter applied to the data
/// </summary>
public record Filter
{
    /// <summary>
    /// The dimension being filtered (e.g., "Region", "Product", "Category")
    /// </summary>
    public required string Dimension { get; init; }
    
    /// <summary>
    /// The value to filter by
    /// </summary>
    public required string Value { get; init; }
}

/// <summary>
/// Exception thrown when intent parsing fails
/// </summary>
public class IntentParsingException : Exception
{
    public IntentParsingException(string message) : base(message) { }
    public IntentParsingException(string message, Exception innerException) : base(message, innerException) { }
}
