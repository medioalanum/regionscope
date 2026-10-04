# RegionScope

RegionScope is a C#/.NET 8 REST API for querying and comparing public indicators for European Union countries. Data is retrieved from Eurostat, normalized, and stored in PostgreSQL.

## Live project

**Public API:** [https://regionscope.onrender.com](https://regionscope.onrender.com)

Quick links:

- [Health](https://regionscope.onrender.com/health) — process status;
- [Readiness](https://regionscope.onrender.com/ready) — database connectivity status;
- [Countries](https://regionscope.onrender.com/api/countries) — catalog of the 27 EU countries;
- [Comparison](https://regionscope.onrender.com/api/compare?countries=IT,DE,FR&indicator=population) — population data for Italy, Germany, and France.

## Overview

The system maintains a catalog of European Union countries and historical series for:

- population;
- GDP per capita;
- unemployment rate;
- the latest ten years available from Eurostat.

In addition to country-level queries, the API provides multi-country comparisons and a protected process for importing updated Eurostat data. Persistence uses Entity Framework Core migrations, and imports are idempotent.

## Architecture and technology stack

- C# and .NET 8;
- ASP.NET Core Minimal API;
- Entity Framework Core 8;
- PostgreSQL with Npgsql;
- Eurostat Statistics API;
- xUnit integration tests;
- Docker Compose for local development;
- GitHub Actions for builds, migrations, and tests;
- Render for API hosting;
- Neon for production PostgreSQL.

## Endpoints

```text
GET  /health
GET  /ready
GET  /api/countries
GET  /api/countries/{code}
GET  /api/countries/{code}/indicators/{indicator}
GET  /api/compare?countries=IT,DE,FR&indicator=unemployment_rate
POST /api/admin/import
```

The administrative endpoint requires the `X-Import-Key` header and is only operational when `Import__ApiKey` is configured. Scheduled imports are optional and disabled by default. To enable them, set `ImportSchedule__Enabled=true` and optionally configure `ImportSchedule__IntervalHours` (default: 24).

## Repository structure

```text
regionscope/
├── src/RegionScope/
│   ├── Clients/       # Eurostat integration
│   ├── Data/          # DbContext and migrations
│   ├── Models/        # Countries, indicators, and observations
│   ├── Services/      # Manual and scheduled imports
│   └── Program.cs     # Configuration and endpoints
├── tests/RegionScope.Tests/
├── .github/workflows/ci.yml
├── Dockerfile
├── compose.yaml
├── RegionScope.sln
└── README.md
```

## Local development

Requirements: .NET 8 SDK and Docker with Compose.

```bash
docker compose up -d postgres
dotnet tool restore
dotnet restore
dotnet ef database update --project src/RegionScope --startup-project src/RegionScope
dotnet run --project src/RegionScope
```

To run a manual local import:

```bash
export Import__ApiKey="a-local-key"
export IMPORT_API_KEY="$Import__ApiKey"
curl -X POST \
  -H "X-Import-Key: $IMPORT_API_KEY" \
  http://localhost:5000/api/admin/import
```

The CI pipeline starts a temporary PostgreSQL instance, applies migrations, and runs the build and tests on .NET 8.

## Data source

Data comes from the [Eurostat Statistics API](https://ec.europa.eu/eurostat/web/user-guides/data-browser/api-data-access/api-getting-started/api). Each observation records the indicator, country, year, unit, source, and retrieval timestamp.

## License

No license has been selected. Until a license is added, all rights remain reserved by the copyright holder.
