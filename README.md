# RegionScope

RegionScope é uma API REST em C#/.NET 8 para consultar e comparar indicadores públicos dos países da União Europeia. Os dados são obtidos do Eurostat, normalizados e armazenados em PostgreSQL.

## Projeto em produção

**API pública:** [https://regionscope.onrender.com](https://regionscope.onrender.com)

Endpoints rápidos:

- [Health](https://regionscope.onrender.com/health) — status do processo;
- [Readiness](https://regionscope.onrender.com/ready) — status da conexão com o banco;
- [Países](https://regionscope.onrender.com/api/countries) — catálogo dos 27 países da UE;
- [Comparação](https://regionscope.onrender.com/api/compare?countries=IT,DE,FR&indicator=population) — população de Itália, Alemanha e França.

## Overview

O sistema mantém um catálogo dos países da União Europeia e séries históricas dos seguintes indicadores:

- população;
- PIB per capita;
- taxa de desemprego;
- últimos dez anos disponíveis no Eurostat.

Além das consultas por país, a API oferece comparação entre múltiplos países e um processo protegido para importar dados atualizados do Eurostat. A persistência usa migrations do Entity Framework Core e as importações são idempotentes.

## Arquitetura e stack

- C# e .NET 8;
- ASP.NET Core Minimal API;
- Entity Framework Core 8;
- PostgreSQL com Npgsql;
- Eurostat Statistics API;
- xUnit e testes de integração;
- Docker Compose para desenvolvimento local;
- GitHub Actions para build, migrations e testes;
- Render para hospedagem da API;
- Neon para PostgreSQL em produção.

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

O endpoint administrativo exige o header `X-Import-Key` e só fica operacional quando `Import__ApiKey` está configurada. A importação agendada é opcional e permanece desativada por padrão; para ativá-la, configure `ImportSchedule__Enabled=true` e, opcionalmente, `ImportSchedule__IntervalHours` (padrão: 24).

## Estrutura do repositório

```text
regionscope/
├── src/RegionScope/
│   ├── Clients/       # Integração com o Eurostat
│   ├── Data/          # DbContext e migrations
│   ├── Models/        # Países, indicadores e observações
│   ├── Services/      # Importação manual e agendada
│   └── Program.cs     # Configuração e endpoints
├── tests/RegionScope.Tests/
├── .github/workflows/ci.yml
├── Dockerfile
├── compose.yaml
├── RegionScope.sln
└── README.md
```

## Desenvolvimento local

Requisitos: .NET 8 SDK e Docker com Compose.

```bash
docker compose up -d postgres
dotnet tool restore
dotnet restore
dotnet ef database update --project src/RegionScope --startup-project src/RegionScope
dotnet run --project src/RegionScope
```

Para executar uma importação manual localmente:

```bash
export Import__ApiKey="uma-chave-local"
export IMPORT_API_KEY="$Import__ApiKey"
curl -X POST \
  -H "X-Import-Key: $IMPORT_API_KEY" \
  http://localhost:5000/api/admin/import
```

O pipeline de CI sobe um PostgreSQL temporário, aplica as migrations e executa build e testes em .NET 8.

## Fonte dos dados

Os dados vêm da [Eurostat Statistics API](https://ec.europa.eu/eurostat/web/user-guides/data-browser/api-data-access/api-getting-started/api). Cada observação registra indicador, país, ano, unidade, fonte e data de recuperação.

## Licença

Nenhuma licença foi selecionada. Até que uma licença seja adicionada, todos os direitos permanecem reservados ao detentor dos direitos autorais.
