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
    /// The original query text
    /// </summary>
    public required string OriginalQuery { get; init; }
}

/// <summary>
/// Represents a time range for data analysis
/// </summary>
public record TimeRange
{
    /// <summary>
    /// Start date of the time range
    /// </summary>
    public required DateTime StartDate { get; init; }
    
    /// <summary>
    /// End date of the time range
    /// </summary>
    public required DateTime EndDate { get; init; }
    
    /// <summary>
    /// Human-readable description of the time range (e.g., "2024", "Last 6 months")
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
    /// The value(s) to filter by
    /// </summary>
    public required List<string> Values { get; init; }
    
    /// <summary>
    /// The operator for the filter (e.g., "equals", "contains", "in")
    /// </summary>
    public required string Operator { get; init; }
}

/// <summary>
/// Valid measures that can be requested
/// </summary>
public static class ValidMeasures
{
    public static readonly HashSet<string> Measures = new(StringComparer.OrdinalIgnoreCase)
    {
        "Revenue",
        "Sales",
        "Profit",
        "Profit Margin",
        "Units Sold",
        "Customer Count",
        "Orders",
        "Average Order Value",
        "Cost",
        "Expenses"
    };
}

/// <summary>
/// Valid time grains for data aggregation
/// </summary>
public static class ValidGrains
{
    public static readonly HashSet<string> Grains = new(StringComparer.OrdinalIgnoreCase)
    {
        "Day",
        "Week",
        "Month",
        "Quarter",
        "Year"
    };
}

/// <summary>
/// Valid dimensions that can be filtered
/// </summary>
public static class ValidDimensions
{
    public static readonly HashSet<string> Dimensions = new(StringComparer.OrdinalIgnoreCase)
    {
        "Region",
        "Country",
        "State",
        "City",
        "Product",
        "Category",
        "Subcategory",
        "Customer",
        "Sales Rep",
        "Channel"
    };
}
