#!/bin/bash
# Bootstrap AskPBI solution structure

# Create solution
dotnet new sln -n AskPBI

# API project
dotnet new webapi -n AskPBI.Api
dotnet sln add AskPBI.Api/AskPBI.Api.csproj

# Bot project (console template as placeholder — we’ll swap to BotFramework later)
dotnet new console -n AskPBI.Bot
dotnet sln add AskPBI.Bot/AskPBI.Bot.csproj

# Shared library for DTOs, utilities
dotnet new classlib -n AskPBI.Shared
dotnet sln add AskPBI.Shared/AskPBI.Shared.csproj

# Charting utility (separate lib so it’s swappable)
dotnet new classlib -n AskPBI.Charting
dotnet sln add AskPBI.Charting/AskPBI.Charting.csproj

# Test project
dotnet new xunit -n AskPBI.Tests
dotnet sln add AskPBI.Tests/AskPBI.Tests.csproj

# Add references
dotnet add AskPBI.Api/AskPBI.Api.csproj reference AskPBI.Shared/AskPBI.Shared.csproj
dotnet add AskPBI.Api/AskPBI.Api.csproj reference AskPBI.Charting/AskPBI.Charting.csproj
dotnet add AskPBI.Bot/AskPBI.Bot.csproj reference AskPBI.Shared/AskPBI.Shared.csproj
dotnet add AskPBI.Tests/AskPBI.Tests.csproj reference AskPBI.Api/AskPBI.Api.csproj

# Docs folder
mkdir docs
touch docs/setup.md docs/demo-script.md docs/cards.json

echo "✅ Project structure created. Next steps:"
echo "1. Open AskPBI.sln in VS Code / Cursor."
echo "2. Run 'dotnet build' to confirm."
echo "3. Start filling in API + Bot logic."
