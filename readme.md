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

1. **Bot Service** (C#, Bot Framework)

   - Connects Teams to the API
   - Handles message extension queries
   - Sends Adaptive Cards back

2. **API Service** (ASP.NET Core)

   - Calls Azure OpenAI for intent extraction + summarization
   - Uses Power BI REST API to push/query data
   - Generates sparkline images (ImageSharp)
   - Assembles Adaptive Card payloads

3. **Shared Libraries**
   - DTOs, card templates, chart utilities

---

## 🛠️ Tech Stack

- **Languages**: C# (.NET 8)
- **AI**: Azure OpenAI (Chat Completions API)
- **Data**: Power BI REST API (push datasets, query)
- **Integrations**: Microsoft Teams (Bot Framework, Message Extensions)
- **Frontend**: Adaptive Cards in Teams
- **Utilities**: SixLabors.ImageSharp (charting), xUnit (tests)

---

## 📂 Project Structure

```plaintext
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
1. User asks a question in Teams (message extension).
2. Bot sends the query to the API.
3. API calls Azure OpenAI → gets structured intent.
4. API fetches/aggregates data from Power BI.
5. API generates a chart + summary.
6. API returns an Adaptive Card → Bot displays it in Teams.

---

## 🎬 Demo Script (90 seconds)
1. In Teams, type: *“Revenue by month in 2024 (NA region)”*
2. Card appears with:
   - Total revenue KPI
   - Sparkline trend
   - AI-written summary
   - Button: “Open in Power BI”
3. Click “Breakdown by region” button → Card refreshes with new insight.

---

## 📈 Why This Project Matters
- **Microsoft-first stack** (Teams, Power BI, Azure OpenAI)
- Demonstrates **AI + business data integration**, a top priority at Microsoft today
- Built in **C#/.NET**, aligning with Microsoft engineering practices
- Shows ability to work across **cloud APIs, AI services, and collaboration tools**

---

## 🚧 Status
- [x] Repo + solution structure
- [x] API skeleton with sample card
- [ ] Azure OpenAI integration
- [ ] Power BI push dataset + aggregation
- [ ] Teams bot wired to return insights

---

## 📜 License
MIT
```
