# Arquitetura da Solução — Atlas ITAM

> **Projeto:** Atlas ITAM — Enterprise IT Asset Lifecycle Platform
> **Versão:** 2.0
> **Data:** 2026-07-17
> **Status:** Rascunho
> **Padrão Arquitetural:** CQRS (Command Query Responsibility Segregation)

---

## 1. Visão Geral da Arquitetura

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENTE                                   │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │  Next.js (Styled Components)                                │ │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐       │ │
│  │  │Dashboard │ │ Ativos   │ │Solicita- │ │ Auditoria│       │ │
│  │  │  Page    │ │  Page    │ │ção Page  │ │  Page    │       │ │
│  │  └──────────┘ └──────────┘ └──────────┘ └──────────┘       │ │
│  │  ┌──────────────────────────────────────────────────────┐   │ │
│  │  │  Services (Axios) → API Calls                         │   │ │
│  │  └──────────────────────────────────────────────────────┘   │ │
│  └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ HTTPS (REST API)
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    ASP.NET Core Web API                          │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │  Controllers (simplified via MediatR)                        │ │
│  └──────────────────────────┬──────────────────────────────────┘ │
│                             │                                    │
│  ┌──────────────────────────▼──────────────────────────────────┐ │
│  │  Middleware Pipeline                                         │ │
│  │  ExceptionHandling → CorrelationId → JwtUser → RateLimiting │ │
│  └──────────────────────────┬──────────────────────────────────┘ │
│                             │                                    │
│  ┌──────────────────────────▼──────────────────────────────────┐ │
│  │  MediatR Pipeline                                            │ │
│  │  ValidationBehavior → AuthorizationBehavior → Handler        │ │
│  └──────────────────────────┬──────────────────────────────────┘ │
│                             │                                    │
│  ┌──────────────────────────▼──────────────────────────────────┐ │
│  │  Application Layer (CQRS)                                    │ │
│  │  ┌────────────────┐    ┌────────────────┐                   │ │
│  │  │   Commands     │    │    Queries     │                   │ │
│  │  │  (Write Ops)   │    │   (Read Ops)   │                   │ │
│  │  └────────────────┘    └────────────────┘                   │ │
│  └──────────────────────────┬──────────────────────────────────┘ │
│                             │                                    │
│  ┌──────────────────────────▼──────────────────────────────────┐ │
│  │  Domain Layer (Entities, Value Objects, Enums, Interfaces)   │ │
│  └──────────────────────────┬──────────────────────────────────┘ │
│                             │                                    │
│  ┌──────────────────────────▼──────────────────────────────────┐ │
│  │  Infrastructure Layer                                        │ │
│  │  ┌────────────┐ ┌────────────┐ ┌────────────┐               │ │
│  │  │ EF Core    │ │ JWT Auth   │ │ PDF Gen    │               │ │
│  │  │ PostgreSQL │ │            │ │            │               │ │
│  │  └────────────┘ └────────────┘ └────────────┘               │ │
│  └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                     PostgreSQL Database                          │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐           │
│  │  Assets  │ │ Requests │ │ Moviment.│ │ AuditLog │           │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘           │
└─────────────────────────────────────────────────────────────────┘
```

---

## 2. Camadas do Backend

| Camada | Projeto | Responsabilidade |
|--------|---------|------------------|
| **Domain** | Atlas.Itam.Domain | Entidades (sealed), Value Objects, Enums, Interfaces de repositório |
| **Application** | Atlas.Itam.Application | Commands, Queries, Handlers, DTOs, Validators, Behaviors |
| **Infrastructure** | Atlas.Itam.Infrastructure | EF Core, Repositories, JWT, PDF, Email |
| **API** | Atlas.Itam.Api | Controllers (via MediatR), Middleware, DI, Program.cs |

---

## 3. Estrutura de Pastas

### 3.1 Backend

```
backend/
├── Atlas.Itam.sln
├── src/
│   ├── Atlas.Itam.Domain/
│   │   ├── Entities/
│   │   │   ├── Asset.cs
│   │   │   ├── AssetCategory.cs
│   │   │   ├── AssetMovement.cs
│   │   │   ├── Request.cs
│   │   │   ├── User.cs
│   │   │   ├── Department.cs
│   │   │   ├── Location.cs
│   │   │   └── AuditLog.cs
│   │   ├── ValueObjects/
│   │   │   ├── PatrimonyNumber.cs
│   │   │   ├── SerialNumber.cs
│   │   │   └── Money.cs
│   │   ├── Enums/
│   │   │   ├── AssetStatus.cs
│   │   │   ├── MovementType.cs
│   │   │   ├── RequestStatus.cs
│   │   │   ├── UserRole.cs
│   │   │   └── AuditAction.cs
│   │   └── Interfaces/
│   │       ├── IAssetRepository.cs
│   │       ├── IRequestRepository.cs
│   │       ├── IUserRepository.cs
│   │       └── IAuditRepository.cs
│   │
│   ├── Atlas.Itam.Application/
│   │   ├── Commands/
│   │   │   ├── Auth/
│   │   │   │   ├── Login/
│   │   │   │   │   ├── LoginCommand.cs
│   │   │   │   │   ├── LoginCommandHandler.cs
│   │   │   │   │   └── LoginCommandValidator.cs
│   │   │   │   ├── RefreshToken/
│   │   │   │   └── Logout/
│   │   │   ├── Assets/
│   │   │   │   ├── CreateAsset/
│   │   │   │   │   ├── CreateAssetCommand.cs
│   │   │   │   │   ├── CreateAssetCommandHandler.cs
│   │   │   │   │   └── CreateAssetCommandValidator.cs
│   │   │   │   ├── UpdateAsset/
│   │   │   │   ├── DeleteAsset/
│   │   │   │   └── TransferAsset/
│   │   │   ├── Requests/
│   │   │   │   ├── CreateRequest/
│   │   │   │   ├── ApproveRequest/
│   │   │   │   ├── RejectRequest/
│   │   │   │   ├── DeliverRequest/
│   │   │   │   └── ReturnRequest/
│   │   │   ├── Categories/
│   │   │   │   ├── CreateCategory/
│   │   │   │   └── UpdateCategory/
│   │   │   └── Users/
│   │   │       ├── CreateUser/
│   │   │       ├── UpdateUser/
│   │   │       └── DeactivateUser/
│   │   │
│   │   ├── Queries/
│   │   │   ├── Assets/
│   │   │   │   ├── GetAsset/
│   │   │   │   │   ├── GetAssetQuery.cs
│   │   │   │   │   └── GetAssetQueryHandler.cs
│   │   │   │   ├── ListAssets/
│   │   │   │   │   ├── ListAssetsQuery.cs
│   │   │   │   │   └── ListAssetsQueryHandler.cs
│   │   │   │   └── SearchAssets/
│   │   │   ├── Requests/
│   │   │   │   ├── GetRequest/
│   │   │   │   ├── ListRequests/
│   │   │   │   └── ListPendingApprovals/
│   │   │   ├── Categories/
│   │   │   │   └── ListCategories/
│   │   │   ├── Users/
│   │   │   │   ├── GetUser/
│   │   │   │   └── ListUsers/
│   │   │   ├── Dashboard/
│   │   │   │   ├── GetDashboardKpis/
│   │   │   │   └── GetWarrantyAlerts/
│   │   │   └── Audit/
│   │   │       └── ListAuditLogs/
│   │   │
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   │   ├── ICommand.cs
│   │   │   │   ├── ICommandHandler.cs
│   │   │   │   ├── IQuery.cs
│   │   │   │   ├── IQueryHandler.cs
│   │   │   │   └── IResult.cs
│   │   │   ├── Behaviors/
│   │   │   │   ├── ValidationBehavior.cs
│   │   │   │   └── LoggingBehavior.cs
│   │   │   ├── Results/
│   │   │   │   └── Result.cs
│   │   │   └── Mappings/
│   │   │       ├── MappingProfile.cs
│   │   │       └── PagedResult.cs
│   │   │
│   │   └── DTOs/
│   │       ├── Assets/
│   │       │   ├── AssetDto.cs
│   │       │   └── AssetSummaryDto.cs
│   │       ├── Requests/
│   │       │   ├── RequestDto.cs
│   │       │   └── RequestSummaryDto.cs
│   │       ├── Users/
│   │       │   ├── UserDto.cs
│   │       │   └── AuthDto.cs
│   │       └── Dashboard/
│   │           └── DashboardDto.cs
│   │
│   ├── Atlas.Itam.Infrastructure/
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs
│   │   │   └── Configurations/
│   │   │       ├── AssetConfiguration.cs
│   │   │       ├── UserConfiguration.cs
│   │   │       └── ... (uma por entidade)
│   │   ├── Repositories/
│   │   │   ├── AssetRepository.cs
│   │   │   ├── RequestRepository.cs
│   │   │   └── ... (uma por interface)
│   │   ├── Auth/
│   │   │   └── JwtTokenService.cs
│   │   └── Services/
│   │       └── PdfService.cs
│   │
│   └── Atlas.Itam.Api/
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── AssetsController.cs
│       │   ├── RequestsController.cs
│       │   ├── DashboardController.cs
│       │   ├── AuditController.cs
│       │   └── UsersController.cs
│       ├── Middleware/
│       │   ├── ExceptionHandlingMiddleware.cs
│       │   ├── JwtUserMiddleware.cs
│       │   └── CorrelationIdMiddleware.cs
│       ├── Extensions/
│       │   └── ServiceCollectionExtensions.cs
│       ├── Program.cs
│       └── appsettings.json
│
├── tests/
│   ├── Atlas.Itam.Domain.Tests/
│   ├── Atlas.Itam.Application.Tests/
│   └── Atlas.Itam.Integration.Tests/
```

### 3.2 Frontend

```
frontend/
├── package.json
├── next.config.js
├── src/
│   ├── app/
│   │   ├── layout.tsx            # Root layout + ThemeProvider
│   │   ├── page.tsx              # Dashboard
│   │   ├── login/
│   │   │   └── page.tsx
│   │   ├── assets/
│   │   │   ├── page.tsx          # Lista de ativos
│   │   │   ├── [id]/
│   │   │   │   └── page.tsx      # Detalhes do ativo
│   │   │   └── new/
│   │   │       └── page.tsx      # Criar ativo
│   │   ├── requests/
│   │   │   ├── page.tsx          # Lista de solicitações
│   │   │   └── new/
│   │   │       └── page.tsx      # Criar solicitação
│   │   └── audit/
│   │       └── page.tsx          # Logs de auditoria
│   ├── components/
│   │   ├── Sidebar/
│   │   │   └── index.tsx
│   │   ├── Header/
│   │   │   └── index.tsx
│   │   ├── Table/
│   │   │   └── index.tsx
│   │   ├── Card/
│   │   │   └── index.tsx
│   │   ├── Modal/
│   │   │   └── index.tsx
│   │   └── Form/
│   │       └── index.tsx
│   ├── hooks/
│   │   ├── useAssets.ts
│   │   ├── useAuth.ts
│   │   └── useRequests.ts
│   ├── services/
│   │   ├── api.ts                # Axios instance
│   │   ├── assets.service.ts
│   │   ├── auth.service.ts
│   │   └── requests.service.ts
│   ├── types/
│   │   ├── asset.ts
│   │   ├── request.ts
│   │   └── user.ts
│   ├── styles/
│   │   ├── global.ts             # Global styles
│   │   ├── theme.ts              # Theme provider
│   │   └── breakpoints.ts        # Responsive breakpoints
│   └── utils/
│       ├── formatDate.ts
│       └── formatCurrency.ts
```

---

## 4. Stack Tecnológica

| Camada | Tecnologia | Versão |
|--------|-----------|--------|
| Runtime | .NET | 8 |
| Framework | ASP.NET Core | 8 |
| ORM | Entity Framework Core | 8 |
| Banco | PostgreSQL | 14+ |
| Auth | JWT (System.IdentityModel) | 7 |
| **Mediator** | **MediatR** | **12.2.0** |
| **Mapeamento** | **AutoMapper** | **12.0.1** |
| Validação | FluentValidation | 11 |
| PDF | QuestPDF | — |
| Frontend | Next.js | 14+ |
| UI | Styled Components | 6 |
| HTTP Client | Axios | 1 |
| State | React Query (TanStack Query) | 5 |
| Forms | React Hook Form + Zod | — |

---

## 5. Decisões de Arquitetura

| Decisão | Escolha | Justificativa |
|---------|---------|---------------|
| **Estilo** | **CQRS com MediatR** | Separação clara entre leitura e escrita, testabilidade, escalabilidade |
| **Mediator** | **MediatR** | Padrão da indústria, grande comunidade, pipeline behaviors |
| **Mapeamento** | **AutoMapper** | Configuração via profiles, reduz código boilerplate |
| **Validação** | **Pipeline Behavior** | Validação automática antes do handler, separação de responsabilidades |
| **Database** | PostgreSQL único | CQRS sem separação Read/Write, simpleridade operacional |
| Comunicação | REST API | Simples, amplamente suportado |
| Auth | JWT (access + refresh) | Stateless, escalável |
| ORM | EF Core | Nativo do .NET, bom suporte a PostgreSQL |
| PDF | QuestPDF | Moderno, fluent API, boa documentação |
| **Classes** | **Sealed por padrão** | Performance (evita vtable), imutabilidade |
| **Variáveis** | **`var` quando tipo óbvio** | Código mais limpo, refatoração mais fácil |
| State Frontend | React Query | Cache automático, re-fetch, loading states |
| Forms | React Hook Form + Zod | Performance, type-safety |
| CSS | Styled Components | CSS-in-JS, theming, responsividade |

---

## 6. Padrão CQRS

### 6.1 Conceito

CQRS separa operações de **leitura** (Queries) de operações de **escrita** (Commands):

```
┌─────────────────────────────────────────────────────────────────┐
│                    CQRS Pattern                                  │
│                                                                  │
│  ┌─────────────────────┐    ┌─────────────────────┐             │
│  │      COMMANDS       │    │       QUERIES        │             │
│  │   (Write Operations)│    │   (Read Operations)  │             │
│  │                     │    │                      │             │
│  │  • CreateAsset      │    │  • GetAsset          │             │
│  │  • UpdateAsset      │    │  • ListAssets        │             │
│  │  • DeleteAsset      │    │  • SearchAssets      │             │
│  │  • ApproveRequest   │    │  • GetDashboardKpis  │             │
│  │  • etc.             │    │  • etc.              │             │
│  └──────────┬──────────┘    └──────────┬──────────┘             │
│             │                          │                        │
│             ▼                          ▼                        │
│  ┌─────────────────────┐    ┌─────────────────────┐             │
│  │   CommandHandler    │    │    QueryHandler      │             │
│  │   (Write to DB)     │    │   (Read from DB)     │             │
│  └──────────┬──────────┘    └──────────┬──────────┘             │
│             │                          │                        │
│             └──────────┬───────────────┘                        │
│                        │                                        │
│                        ▼                                        │
│              ┌─────────────────┐                                │
│              │    Database     │                                │
│              │   (PostgreSQL)  │                                │
│              └─────────────────┘                                │
└─────────────────────────────────────────────────────────────────┘
```

### 6.2 Fluxo de uma Requisição

```
HTTP Request
    │
    ▼
ExceptionHandlingMiddleware (catch global)
    │
    ▼
CorrelationIdMiddleware (gera X-Correlation-Id)
    │
    ▼
JwtUserMiddleware (extrai userId do token)
    │
    ▼
Rate Limiting (Token Bucket por IP)
    │
    ▼
Controller (recebe request, cria Command/Query)
    │
    ▼
MediatR.Send(Command/Query)
    │
    ▼
ValidationBehavior (Pipeline Behavior)
    │  └─ FluentValidation valida o Command/Query
    ▼
LoggingBehavior (Pipeline Behavior)
    │  └─ Loga início/fim da operação
    ▼
Handler (executa lógica de negócio)
    │
    ▼
Repository (acesso a dados via EF Core)
    │
    ▼
PostgreSQL
```

### 6.3 Interfaces Base

```csharp
// Comando (escrita)
public interface ICommand<out TResponse> : IRequest<TResponse> { }

// Handler de comando
public interface ICommandHandler<in TCommand, TResponse> 
    : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> { }

// Query (leitura)
public interface IQuery<out TResponse> : IRequest<TResponse> { }

// Handler de query
public interface IQueryHandler<in TQuery, TResponse> 
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse> { }

// Resultado padronizado
public interface IResult<out T>
{
    bool IsSuccess { get; }
    T? Value { get; }
    string? Error { get; }
}
```

### 6.4 Exemplo: Criar Ativo

#### Command
```csharp
public sealed record CreateAssetCommand(
    string Name,
    string PatrimonyNumber,
    string SerialNumber,
    DateTime AcquisitionDate,
    decimal AcquisitionValue,
    string? Supplier,
    DateTime? WarrantyUntil,
    Guid CategoryId,
    Guid LocationId,
    Guid? CurrentUserId
) : ICommand<Guid>;
```

#### Validator
```csharp
public sealed class CreateAssetCommandValidator 
    : AbstractValidator<CreateAssetCommand>
{
    public CreateAssetCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200);

        RuleFor(x => x.PatrimonyNumber)
            .NotEmpty().WithMessage("Patrimônio é obrigatório")
            .MaximumLength(50);

        RuleFor(x => x.AcquisitionValue)
            .GreaterThan(0).WithMessage("Valor deve ser maior que 0");
    }
}
```

#### Handler
```csharp
public sealed class CreateAssetCommandHandler(
    IAssetRepository repository,
    IMapper mapper
) : ICommandHandler<CreateAssetCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateAssetCommand request,
        CancellationToken cancellationToken)
    {
        var asset = mapper.Map<Asset>(request);
        
        var id = await repository.AddAsync(asset, cancellationToken);
        
        return Result.Success(id);
    }
}
```

#### Controller
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AssetsController(
    IMediator mediator
) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "admin,it_manager")]
    public async Task<IActionResult> Create(
        [FromBody] CreateAssetCommand command)
    {
        var result = await mediator.Send(command);
        
        if (result.IsSuccess)
            return CreatedAtAction(
                nameof(GetById), 
                new { id = result.Value }, 
                result.Value);
        
        return BadRequest(result.Error);
    }
}
```

---

## 7. Convenções de Código

### 7.1 Classes Seladas

**Todas as classes devem ser declaradas como `sealed` por padrão:**

```csharp
// ✓ CORRETO
public sealed class Asset { }
public sealed record CreateAssetCommand(...) : ICommand<Guid> { }
public sealed class CreateAssetCommandHandler { }
public sealed class AssetDto { }

// ✗ EVITAR (exceto herança necessária)
public class Asset { }
```

**Exceções (podem ter herança):**
- Validators do FluentValidation (`AbstractValidator<T>`)
- Entities que usam herança (se necessário)
- Classes abstratas (`abstract class`)

### 7.2 Uso de `var`

**Convenção:** Usar `var` quando o tipo é óbvio na declaração:

```csharp
// ✓ BOM - tipo óbvio
var asset = new Asset();
var assets = new List<Asset>();
var command = new CreateAssetCommand();
var result = await mediator.Send(command);

// ✗ EVITAR - tipo não óbvio
Asset asset = GetAssetById(id);  // método retorna Asset
string name = asset.Name;        // propriedade é string
int count = assets.Count;        // propriedade é int
```

### 7.3 Naming Conventions

| Elemento | Convenção | Exemplo |
|----------|-----------|---------|
| Commands | `{Verb}{Noun}Command` | `CreateAssetCommand` |
| Queries | `{Verb}{Noun}Query` | `GetAssetQuery`, `ListAssetsQuery` |
| Handlers | `{Command/Query}Handler` | `CreateAssetCommandHandler` |
| DTOs | `{Entity}Dto` | `AssetDto`, `RequestDto` |
| Validators | `{Command}Validator` | `CreateAssetCommandValidator` |
| Entities | `{Noun}` (singular) | `Asset`, `Request`, `User` |

---

## 8. Dependências NuGet

### 8.1 Atlas.Itam.Application.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AutoMapper" Version="12.0.1" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
    <PackageReference Include="FluentValidation" Version="11.9.0" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
    <PackageReference Include="MediatR" Version="12.2.0" />
    <PackageReference Include="MediatR.Extensions.FluentValidation.AspNetCore" Version="2.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Atlas.Itam.Domain\Atlas.Itam.Domain.csproj" />
  </ItemGroup>
</Project>
```

### 8.2 Atlas.Itam.Api.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.2.0" />
    <PackageReference Include="Serilog.AspNetCore" Version="7.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Atlas.Itam.Application\Atlas.Itam.Application.csproj" />
    <ProjectReference Include="..\Atlas.Itam.Infrastructure\Atlas.Itam.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

---

## 9. Configuração do MediatR

### 9.1 ServiceCollectionExtensions.cs

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAtlasItam(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // MediatR
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        
        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        
        // FluentValidation with Pipeline Behavior
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddValidatorsFromAssembly(typeof(CreateAssetCommandValidator).Assembly);
        
        // Repositories
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        
        // Infrastructure Services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPdfService, PdfService>();
        
        return services;
    }
}
```

---

## 10. Endpoints da API

| Método | Rota | Command/Query | Auth |
|--------|------|---------------|------|
| POST | `/api/auth/login` | `LoginCommand` | Não |
| POST | `/api/auth/refresh` | `RefreshTokenCommand` | Não |
| POST | `/api/auth/logout` | `LogoutCommand` | Sim |
| GET | `/api/assets` | `ListAssetsQuery` | Sim |
| POST | `/api/assets` | `CreateAssetCommand` | Sim (Admin/ITManager) |
| GET | `/api/assets/{id}` | `GetAssetQuery` | Sim |
| PUT | `/api/assets/{id}` | `UpdateAssetCommand` | Sim (Admin/ITManager) |
| DELETE | `/api/assets/{id}` | `DeleteAssetCommand` | Sim (Admin) |
| GET | `/api/assets/{id}/movements` | `ListAssetMovementsQuery` | Sim |
| GET | `/api/categories` | `ListCategoriesQuery` | Sim |
| POST | `/api/categories` | `CreateCategoryCommand` | Sim (Admin) |
| GET | `/api/requests` | `ListRequestsQuery` | Sim |
| POST | `/api/requests` | `CreateRequestCommand` | Sim |
| GET | `/api/requests/{id}` | `GetRequestQuery` | Sim |
| PUT | `/api/requests/{id}/approve` | `ApproveRequestCommand` | Sim (Manager/Admin) |
| PUT | `/api/requests/{id}/reject` | `RejectRequestCommand` | Sim (Manager/Admin) |
| PUT | `/api/requests/{id}/deliver` | `DeliverRequestCommand` | Sim (ITManager) |
| PUT | `/api/requests/{id}/return` | `ReturnRequestCommand` | Sim (ITManager) |
| GET | `/api/dashboard` | `GetDashboardKpisQuery` | Sim |
| GET | `/api/audit` | `ListAuditLogsQuery` | Sim (Admin/ITManager) |
| GET | `/api/users` | `ListUsersQuery` | Sim (Admin) |
| POST | `/api/users` | `CreateUserCommand` | Sim (Admin) |
| PUT | `/api/users/{id}` | `UpdateUserCommand` | Sim (Admin) |

---

## 11. Variáveis de Ambiente

```bash
# Database
DB_HOST=localhost
DB_PORT=5432
DB_USER=postgres
DB_PASS=minha-senha
DB_NAME=atlas_itam

# JWT
JWT_SECRET=chave-secreta-forte-com-64-bytes-hex
JWT_ISSUER=atlas-itam-api
JWT_AUDIENCE=atlas-itam-frontend
JWT_ACCESS_EXPIRY_MINUTES=15
JWT_REFRESH_EXPIRY_DAYS=7

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5000

# Frontend
NEXT_PUBLIC_API_URL=http://localhost:5000
```

---

## Documentos Relacionados

- `Atlas_ITAM_Requisitos_Funcionais.md` — Requisitos funcionais
- `Atlas_ITAM_Requisitos_Nao_Funcionais.md` — Requisitos não funcionais
- `Atlas_ITAM_Regras_Negocio.md` — Regras de negócio
- `Atlas_ITAM_Modelagem_Dominio.md` — Modelagem de domínio
- `Atlas_ITAM_Backlog.md` — Backlog do produto
- `Atlas_ITAM_Modelagem_Banco.md` — Modelagem do banco de dados