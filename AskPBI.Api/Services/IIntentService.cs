using AskPBI.Api.Models;

namespace AskPBI.Api.Services;

/// <summary>
/// Service interface for parsing natural language queries into structured intent
/// </summary>
public interface IIntentService
{
    /// <summary>
    /// Parses a natural language query into structured intent
    /// </summary>
    /// <param name="query">The natural language query to parse</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The parsed intent result</returns>
    Task<IntentResult> ParseIntentAsync(string query, CancellationToken cancellationToken = default);
}
