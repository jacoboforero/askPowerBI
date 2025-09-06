using Azure.AI.OpenAI;
using AskPBI.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configure Azure OpenAI
builder.Services.Configure<AzureOpenAIOptions>(
    builder.Configuration.GetSection(AzureOpenAIOptions.SectionName));

// Register Azure OpenAI client
builder.Services.AddSingleton<OpenAIClient>(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;
    
    if (string.IsNullOrEmpty(options.Endpoint) || string.IsNullOrEmpty(options.ApiKey))
    {
        throw new InvalidOperationException("Azure OpenAI endpoint and API key must be configured in appsettings.json");
    }
    
    return new OpenAIClient(new Uri(options.Endpoint), new Azure.AzureKeyCredential(options.ApiKey));
});

// Register intent service
builder.Services.AddScoped<IIntentService, AzureOpenAIIntentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Sample Adaptive Card for insights
var sampleCard = new
{
    type = "AdaptiveCard",
    version = "1.4",
    body = new object[]
    {
        new
        {
            type = "TextBlock",
            text = "📊 Revenue Insights",
            size = "Large",
            weight = "Bolder",
            color = "Accent"
        },
        new
        {
            type = "ColumnSet",
            columns = new object[]
            {
                new
                {
                    type = "Column",
                    width = "stretch",
                    items = new object[]
                    {
                        new
                        {
                            type = "TextBlock",
                            text = "Total Revenue",
                            size = "Medium",
                            weight = "Bolder"
                        },
                        new
                        {
                            type = "TextBlock",
                            text = "$2,847,392",
                            size = "ExtraLarge",
                            weight = "Bolder",
                            color = "Good"
                        }
                    }
                },
                new
                {
                    type = "Column",
                    width = "stretch",
                    items = new object[]
                    {
                        new
                        {
                            type = "TextBlock",
                            text = "Growth Rate",
                            size = "Medium",
                            weight = "Bolder"
                        },
                        new
                        {
                            type = "TextBlock",
                            text = "+12.5%",
                            size = "ExtraLarge",
                            weight = "Bolder",
                            color = "Good"
                        }
                    }
                }
            }
        },
        new
        {
            type = "TextBlock",
            text = "📈 Trend Analysis",
            size = "Medium",
            weight = "Bolder",
            margin = new { top = "Medium" }
        },
        new
        {
            type = "Image",
            url = "data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAwIiBoZWlnaHQ9IjEwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KICA8cG9seWxpbmUgZmlsbD0ibm9uZSIgc3Ryb2tlPSIjMDA3OGQ3IiBzdHJva2Utd2lkdGg9IjMiIHBvaW50cz0iMCw4MCAyMCw3MCA0MCw2MCA2MCw1MCA4MCw0MCAxMDAsMzAgMTIwLDIwIDE0MCwxMCAxNjAsMTUgMTgwLDIwIDIwMCwyNSAyMjAsMzAgMjQwLDM1IDI2MCw0MCAyODAsNDUgMzAwLDUwIDMyMCw1NSAzNDAsNjAgMzYwLDY1IDM4MCw3MCA0MDAsNzUiLz4KICA8Y2lyY2xlIGN4PSI0MDAiIGN5PSI3NSIgcj0iNCIgZmlsbD0iIzAwNzhkNyIvPgo8L3N2Zz4K",
            size = "Medium",
            margin = new { top = "Small" }
        },
        new
        {
            type = "TextBlock",
            text = "💡 AI Summary",
            size = "Medium",
            weight = "Bolder",
            margin = new { top = "Medium" }
        },
        new
        {
            type = "TextBlock",
            text = "Revenue has shown consistent growth throughout 2024, with the North American region leading performance. The 12.5% year-over-year increase reflects strong market demand and effective sales strategies. Key drivers include new product launches and expanded market penetration.",
            wrap = true,
            margin = new { top = "Small" }
        }
    },
    actions = new object[]
    {
        new
        {
            type = "Action.OpenUrl",
            title = "Open in Power BI",
            url = "https://app.powerbi.com/groups/me/reports/sample-report",
            style = "positive"
        },
        new
        {
            type = "Action.Submit",
            title = "Breakdown by Region",
            data = new { action = "breakdown", region = "all" }
        }
    }
};

// POST /insights/query endpoint
app.MapPost("/insights/query", async (InsightQueryRequest request, IIntentService intentService) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Received insight query: {Query}", request.Query);
    
    try
    {
        // Parse the natural language query into structured intent
        var intent = await intentService.ParseIntentAsync(request.Query);
        
        logger.LogInformation("Parsed intent: {Intent}", System.Text.Json.JsonSerializer.Serialize(intent));
        
        // For now, return the hardcoded sample card with the parsed intent
        var response = new
        {
            success = true,
            card = sampleCard,
            intent = intent,
            timestamp = DateTime.UtcNow,
            query = request.Query
        };
        
        logger.LogInformation("Returning sample adaptive card with parsed intent for query: {Query}", request.Query);
        
        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to process insight query: {Query}", request.Query);
        
        var errorResponse = new
        {
            success = false,
            error = ex.Message,
            timestamp = DateTime.UtcNow,
            query = request.Query
        };
        
        return Results.BadRequest(errorResponse);
    }
})
.WithName("GetInsights")
.WithOpenApi();

app.Run();

// Request model for the insights endpoint
public record InsightQueryRequest(string Query);
