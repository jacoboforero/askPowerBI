# AskPBI

🚀 **AskPBI** is a C#/.NET project that integrates **Microsoft Teams**, **Power BI**, and **Azure OpenAI** to let users ask natural language questions about their business data and instantly see insights inside Teams.

---

## 🎯 What It Does

- **Natural Language to Insight**  
  Users type questions in Microsoft Teams like _“Revenue by month in 2024, NA region”_.
- **AI-Powered Understanding**  
  Azure OpenAI translates the question into structured intent (metrics, time, filters).
- **Data from Power BI**  
  The backend queries (or pushes demo data to) Power BI using its REST API.
- **Beautiful Results in Teams**  
  An Adaptive Card with:
  - KPI (e.g., total revenue)
  - Sparkline chart (trend over time)
  - LLM-generated summary
  - Button linking to the Power BI workspace

All of this happens **directly inside Teams** — no extra UI to build.

---

## 🧩 Architecture

**Frontend**

- Microsoft Teams (desktop/web/mobile)
- Adaptive Cards as the “UI layer”

**Backend Components**

1. **Bot Service** (C#, Bot Framework) - _Planned_

   - 🔄 Connects Teams to the API
   - 🔄 Handles message extension queries
   - 🔄 Sends Adaptive Cards back

2. **API Service** (ASP.NET Core)

   - ✅ Calls Azure AI Foundry for intent extraction
   - ✅ Generates structured intent from natural language
   - ✅ Returns Adaptive Card payloads with sample data
   - 🔄 Uses Power BI REST API to push/query data - _Planned_
   - 🔄 Generates sparkline images - _Planned_

3. **Shared Libraries** - _Basic structure_
   - 🔄 DTOs, card templates, chart utilities - _Planned_

---

## 🛠️ Tech Stack

- **Languages**: C# (.NET 9)
- **AI**: Azure AI Foundry (GPT-5-mini deployment)
- **Data**: Power BI REST API (push datasets, query) - _Planned_
- **Integrations**: Microsoft Teams (Bot Framework, Message Extensions) - _Planned_
- **Frontend**: Adaptive Cards in Teams
- **Utilities**: xUnit (tests), Swagger/OpenAPI

---

## 📂 Project Structure

````plaintext
.
├── AskPBI.sln                # Solution file
├── AskPBI.Api/               # ASP.NET Core Web API (AI + Power BI integration)
├── AskPBI.Bot/               # Teams bot (message extension handler)
├── AskPBI.Shared/            # DTOs, Adaptive Card builders
├── AskPBI.Charting/          # Chart rendering utilities
├── AskPBI.Tests/             # xUnit tests
└── docs/                     # Documentation
    ├── setup.md              # Azure + Teams + Power BI setup steps
    ├── demo-script.md        # 90-second demo walkthrough
    └── cards.json            # Sample Adaptive Card payloads


---

## 🧪 How It Works (Step by Step)
1. 🔄 User asks a question in Teams (message extension) - *Planned*
2. 🔄 Bot sends the query to the API - *Planned*
3. ✅ API calls Azure AI Foundry → gets structured intent.
4. 🔄 API fetches/aggregates data from Power BI - *Planned*
5. 🔄 API generates a chart + summary - *Planned*
6. ✅ API returns an Adaptive Card with sample data.
7. 🔄 Bot displays it in Teams - *Planned*

**Current Implementation:**
- ✅ Working API endpoint: `POST /insights/query`
- ✅ Natural language to structured intent parsing
- ✅ Sample Adaptive Card generation
- ✅ Swagger documentation at `/swagger`

---

## 🎬 Demo Script (Current API Testing)
1. Start the API: `dotnet run` in `AskPBI.Api/`
2. Open Swagger UI: `http://localhost:5079/swagger`
3. Test the endpoint: `POST /insights/query`
   ```json
   {
     "Query": "Revenue by month in 2024 (NA region)"
   }
````

4. Response includes:
   - ✅ Structured intent (measure, timeRange, grain, filters)
   - ✅ Sample Adaptive Card with revenue data
   - ✅ AI-generated summary

**Future Teams Integration:**

- 🔄 In Teams, type: _"Revenue by month in 2024 (NA region)"_
- 🔄 Card appears with real Power BI data
- 🔄 Click "Breakdown by region" button → Card refreshes with new insight

---

## 📈 Why This Project Matters

- **Microsoft-first stack** (Teams, Power BI, Azure AI Foundry)
- Demonstrates **AI + business data integration**, a top priority at Microsoft today
- Built in **C#/.NET 9**, aligning with Microsoft engineering practices
- Shows ability to work across **cloud APIs, AI services, and collaboration tools**
- **Clean architecture** with proper separation of concerns (generic AI client vs intent-specific logic)
- **Refactored codebase** with improved maintainability and testability

---

## 🚧 Status

- [x] Repo + solution structure
- [x] API with working endpoint (`/insights/query`)
- [x] Azure AI Foundry integration (GPT-5-mini)
- [x] Natural language to structured intent parsing
- [x] Sample Adaptive Card generation
- [x] Swagger/OpenAPI documentation
- [x] Clean architecture with proper separation of concerns
- [x] Comprehensive .gitignore and security practices
- [ ] Power BI REST API integration
- [ ] Real data fetching and aggregation
- [ ] Chart generation (sparklines)
- [ ] Teams bot implementation
- [ ] Message extension integration

---

## 🚀 Quick Start

### Prerequisites

- .NET 9 SDK
- Azure AI Foundry deployment (GPT-5-mini)

### Running the API

```bash
cd AskPBI.Api
dotnet run
```

The API will start on `http://localhost:5079` with Swagger UI available at `/swagger`.

### Testing the Intent Parsing

```bash
curl -X POST http://localhost:5079/insights/query \
  -H "Content-Type: application/json" \
  -d '{"Query": "Profit margin by quarter"}'
```

### Configuration

Update `appsettings.Development.json` with your Azure AI Foundry credentials:

```json
{
  "Foundry": {
    "Endpoint": "https://your-foundry-endpoint.cognitiveservices.azure.com/",
    "ApiKey": "your-api-key",
    "DeploymentName": "your-gpt-5-mini-deployment"
  }
}
```

---

## 📜 License

MIT
