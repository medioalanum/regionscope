# RegionScope

A small, developer-friendly C#/.NET API for exploring and comparing European indicators from Eurostat.

> Project status: planned. This repository is the starting point for a focused portfolio project and does not contain the implementation yet.

## Overview

RegionScope will collect a small set of public European indicators, normalize the source data, store it in PostgreSQL, and expose a clear REST API for querying and comparing countries over time.

The initial scope is intentionally small:

- Population
- GDP per capita
- Unemployment rate
- European Union countries
- The latest ten available years

The project is designed to demonstrate a complete backend workflow in C# and .NET: external API integration, data transformation, persistence, API design, testing, containerization, and deployment.

## Planned stack

- C# and .NET
- ASP.NET Core
- Entity Framework Core with PostgreSQL and Npgsql
- Eurostat Statistics API
- xUnit and Testcontainers
- Docker and GitHub Actions
- Render and Neon for deployment

## Planned API

The first version is expected to provide endpoints such as:

- `GET /health`
- `GET /ready`
- `GET /api/countries`
- `GET /api/countries/{code}`
- `GET /api/countries/{code}/indicators/{indicator}`
- `GET /api/compare?countries=IT,DE,FR&indicator=unemployment_rate`

The API contract will be finalized alongside the implementation.

## Repository structure

The planned structure keeps the application proportional to its scope:

```
regionscope/
├── src/RegionScope/
│   ├── Api/
│   ├── Clients/
│   ├── Data/
│   ├── Models/
│   ├── Services/
│   └── Program.cs
├── tests/RegionScope.Tests/
├── Dockerfile
├── compose.yaml
├── RegionScope.sln
└── README.md
```

## Development roadmap

1. Bootstrap the .NET solution and health endpoint.
2. Add PostgreSQL, EF Core, and the initial data model.
3. Implement country queries and seed data.
4. Integrate the Eurostat client.
5. Add idempotent indicator imports.
6. Expose indicator history and country comparison endpoints.
7. Add tests, Docker support, CI, and deployment documentation.

## Local development

Requirements: .NET 8 SDK and Docker with Compose.

```bash
docker compose up -d postgres
dotnet tool restore
dotnet restore
dotnet ef database update --project src/RegionScope --startup-project src/RegionScope
dotnet run --project src/RegionScope
```

The API runs on the URL shown by ASP.NET Core. Useful endpoints include:

- `GET /health` — process health;
- `GET /ready` — database readiness;
- `GET /api/countries` — EU country catalog;
- `GET /api/countries/IT/indicators/population` — country history;
- `GET /api/compare?countries=IT,DE,FR&indicator=unemployment_rate` — country comparison.

The Eurostat import endpoint is intentionally disabled unless `Import__ApiKey` is configured. When enabled, run it with:

```bash
curl -X POST \\
  -H "X-Import-Key: $IMPORT_API_KEY" \\
  http://localhost:5000/api/admin/import
```

Automatic imports are disabled by default. To enable them in a hosted environment, configure `ImportSchedule__Enabled=true` and optionally set `ImportSchedule__IntervalHours` (default: 24).

Production currently runs on Render and uses Neon PostgreSQL. The production connection string must use Npgsql's key/value format rather than a `postgresql://` URI.

## Data source

The project will use the [Eurostat Statistics API](https://ec.europa.eu/eurostat/web/main/data/web-services) as its initial data source. Stored observations will include retrieval metadata so that the API can make data provenance and freshness explicit.

## Contributing

This is a personal learning and portfolio project. Issues and suggestions are welcome, especially when they improve correctness, clarity, or the quality of the implementation.

## License

No license has been selected yet. Until a license is added, all rights are reserved by the copyright holder.
