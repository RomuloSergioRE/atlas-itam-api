# Atlas ITAM API — Enterprise IT Asset Lifecycle Platform

Backend da plataforma web para gestão do ciclo de vida de ativos de TI (ITAM).

## 🎯 Objetivo

Centralizar o controle de equipamentos, licenças e movimentações, substituindo planilhas por um sistema corporativo com rastreabilidade, aprovações e dashboards.

## 🛠️ Stack Tecnológica

- **C# / .NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core 8**
- **PostgreSQL 14+**
- **MediatR** (CQRS)
- **AutoMapper**
- **FluentValidation**
- **JWT Authentication**

## 📁 Estrutura do Projeto

```
atlas-itam-api/
├── src/
│   ├── Atlas.Itam.Domain/           # Entidades, Value Objects, Enums
│   ├── Atlas.Itam.Application/      # Commands, Queries, Behaviors
│   ├── Atlas.Itam.Infrastructure/   # EF Core, JWT, Services
│   └── Atlas.Itam.Api/             # Controllers, Program.cs
├── tests/
│   ├── Atlas.Itam.Domain.Tests/
│   ├── Atlas.Itam.Application.Tests/
│   └── Atlas.Itam.Integration.Tests/
└── docs/
    ├── database.sql
    └── Atlas_ITAM_*.md
```

## 🚀 Como Executar

### Pré-requisitos
- .NET 8 SDK
- PostgreSQL 14+

### Setup

1. Clone o repositório:
```bash
git clone https://github.com/RomuloSergioRE/atlas-itam-api.git
cd atlas-itam-api
```

2. Crie o banco de dados:
```bash
psql -U postgres -c "CREATE DATABASE atlas_itam;"
```

3. Execute os scripts SQL:
```bash
psql -U postgres -d atlas_itam -f docs/database.sql
```

4. Execute a API:
```bash
cd src/Atlas.Itam.Api
dotnet run
```

5. Acesse o Swagger:
```
http://localhost:5000/swagger
```

## 📚 Documentação

- [Requisitos Funcionais](docs/Atlas_ITAM_Requisitos_Funcionais.md)
- [Requisitos Não Funcionais](docs/Atlas_ITAM_Requisitos_Nao_Funcionais.md)
- [Regras de Negócio](docs/Atlas_ITAM_Regras_Negocio.md)
- [Modelagem de Domínio](docs/Atlas_ITAM_Modelagem_Dominio.md)
- [Modelagem do Banco](docs/Atlas_ITAM_Modelagem_Banco.md)
- [Arquitetura da Solução](docs/Atlas_ITAM_Arquitetura_Solucao.md)
- [Backlog](docs/Atlas_ITAM_Backlog.md)

## 👤 Usuário Padrão

- **Email:** admin@atlasitam.com
- **Senha:** admin123

## 🌿 Branches

- `main` — Produção (código estável)
- `dev` — Desenvolvimento (feature branches)

## 📄 Licença

Projeto privado - Todos os direitos reservados.
