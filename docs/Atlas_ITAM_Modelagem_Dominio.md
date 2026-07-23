# Modelagem de Domínio — Atlas ITAM

> **Projeto:** Atlas ITAM — Enterprise IT Asset Lifecycle Platform
> **Versão:** 2.0
> **Data:** 2026-07-17
> **Status:** Rascunho
> **Padrão Arquitetural:** CQRS com MediatR

---

## 1. Visão Geral

Este documento apresenta a modelagem de domínio do Atlas ITAM, definindo entidades, value objects, agregados e seus relacionamentos. O modelo segue conceitos de Domain-Driven Design (DDD) para garantir coerência e clareza.

---

## 2. Agregados e Entidades

### 2.1 Agregado: Asset (Ativo)

**Raiz do agregado:** `Asset`

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|:-----------:|-----------|
| AssetId | guid | Sim | Identificador único |
| Name | string | Sim | Nome descritivo do equipamento |
| PatrimonyNumber | Value Object | Sim | Número de patrimônio (único no sistema) |
| SerialNumber | Value Object | Sim | Número de série (único no sistema) |
| AcquisitionDate | date | Sim | Data de aquisição |
| AcquisitionValue | Money | Sim | Valor de aquisição (Value Object) |
| Supplier | string | Não | Fornecedor |
| WarrantyUntil | date | Não | Data de término da garantia |
| Status | enum | Sim | Disponível, Em Uso, Em Manutenção, Transferido, Baixado |
| Observations | text | Não | Observações gerais |
| CategoryId | FK | Sim | Referência para AssetCategory |
| LocationId | FK | Sim | Referência para Location |
| CurrentUserId | FK | Nullable | Usuário atual (null quando disponível) |
| IsDeleted | bool | Sim | Soft delete |

**Value Objects:**

| Value Object | Campos | Regras |
|--------------|--------|--------|
| PatrimonyNumber | Value (string) | Único no sistema, não nulo |
| SerialNumber | Value (string) | Único no sistema, não nulo |
| Money | Amount (decimal), Currency (string) | Amount > 0, Currency = "BRL" |

**Enum AssetStatus:**

| Valor | Descrição |
|-------|-----------|
| Available | Disponível para solicitação |
| InUse | Vinculado a um colaborador |
| InMaintenance | Em manutenção técnica |
| Transferred | Em processo de transferência |
| Retired | Baixado do sistema |

---

### 2.2 Agregado: AssetCategory (Categoria)

**Raiz do agregado:** `AssetCategory`

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|:-----------:|-----------|
| CategoryId | guid | Sim | Identificador único |
| Name | string | Sim | Nome da categoria |
| Description | string | Não | Descrição da categoria |
| IsActive | bool | Sim | Se a categoria está ativa |

**Categorias padrão:**

| Nome | Descrição |
|------|-----------|
| Notebook | Computador portátil |
| Desktop | Computador de mesa |
| Monitor | Tela/monitor |
| Celular | Smartphones corporativos |
| Tablet | Tablets corporativos |
| Dock Station | Estações de acoplamento |
| Impressora | Impressoras e multifuncionais |
| Servidor | Servidores físicos |
| Switch | Switches de rede |
| Firewall | Dispositivos de segurança |
| Roteador | Roteadores de rede |
| Licença de Software | Licenças de uso |
| Periférico | Mouses, teclados, headsets, webcams |

---

### 2.3 Agregado: AssetMovement (Movimentação)

**Raiz do agregado:** `AssetMovement`

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|:-----------:|-----------|
| MovementId | guid | Sim | Identificador único |
| Type | enum | Sim | Tipo da movimentação |
| Date | datetime | Sim | Data e hora da movimentação |
| AssetId | FK | Sim | Ativo movimentado |
| FromUserId | FK | Nullable | Usuário de origem (null na primeira entrega) |
| ToUserId | FK | Nullable | Usuário de destino (null na devolução) |
| ResponsibleId | FK | Sim | Quem realizou a movimentação |
| Observation | text | Não | Detalhes da movimentação |
| RequestId | FK | Nullable | Solicitação vinculada (se houver) |

**Enum MovementType:**

| Valor | Descrição | Status do Ativo |
|-------|-----------|-----------------|
| Delivery | Entrega ao colaborador | → InUse |
| Transfer | Transferência entre usuários | → InUse (novo usuário) |
| Return | Devolução do colaborador | → Available |
| Maintenance | Envio para manutenção | → InMaintenance |
| Retirement | Baixa do ativo | → Retired |

---

### 2.4 Agregado: Request (Solicitação)

**Raiz do agregado:** `Request`

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|:-----------:|-----------|
| RequestId | guid | Sim | Identificador único |
| Status | enum | Sim | Status da solicitação |
| Justification | text | Sim | Motivo da solicitação |
| CreatedAt | datetime | Sim | Data de criação |
| UpdatedAt | datetime | Sim | Última atualização |
| RequestedById | FK | Sim | Colaborador que solicitou |
| ApprovedById | FK | Nullable | Gestor que aprovou/rejeitou |
| ApprovedAt | datetime | Nullable | Data da aprovação/rejeição |
| RejectionReason | text | Nullable | Motivo da rejeição |
| AssetId | FK | Sim | Ativo solicitado |

**Enum RequestStatus:**

| Valor | Descrição |
|-------|-----------|
| Pending | Aguardando aprovação do gestor |
| Approved | Aprovada pelo gestor, aguardando entrega |
| Rejected | Rejeitada pelo gestor |
| Delivered | Equipamento entregue ao colaborador |
| Returned | Equipamento devolvido pelo colaborador |

---

### 2.5 Agregado: User (Usuário)

**Raiz do agregado:** `User`

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|:-----------:|-----------|
| UserId | guid | Sim | Identificador único |
| Name | string | Sim | Nome completo |
| Email | string | Sim | E-mail (único no sistema) |
| PasswordHash | string | Sim | Senha com hash (BCrypt/Argon2) |
| Role | enum | Sim | Perfil de acesso |
| DepartmentId | FK | Sim | Departamento vinculado |
| IsActive | bool | Sim | Se o usuário está ativo |
| CreatedAt | datetime | Sim | Data de criação |

**Enum UserRole:**

| Valor | Descrição | Permissões |
|-------|-----------|------------|
| Admin | Administrador do sistema | Total |
| ITManager | Gestor de TI | Ativos, aprovações, auditoria |
| Manager | Gestor de departamento | Aprovações do seu time |
| HR | Recursos Humanos | Visualizar ativos, criar solicitações |
| Facilities | Infraestrutura | Visualizar ativos |

---

### 2.6 Agregado: Department (Departamento)

**Raiz do agregado:** `Department`

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|:-----------:|-----------|
| DepartmentId | guid | Sim | Identificador único |
| Name | string | Sim | Nome do departamento |

---

### 2.7 Agregado: Location (Localização)

**Raiz do agregado:** `Location`

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|:-----------:|-----------|
| LocationId | guid | Sim | Identificador único |
| Name | string | Sim | Nome da localização (ex: "Sede SP - 3º andar") |
| Address | string | Não | Endereço completo |

---

### 2.8 Agregado: AuditLog (Auditoria)

**Raiz do agregado:** `AuditLog`

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|:-----------:|-----------|
| LogId | guid | Sim | Identificador único |
| Action | enum | Sim | Tipo da ação realizada |
| EntityName | string | Sim | Nome da entidade afetada |
| EntityId | guid | Sim | Identificador do registro afetado |
| UserId | FK | Sim | Usuário que realizou a ação |
| Timestamp | datetime | Sim | Data e hora da ação |
| OldValues | json | Nullable | Dados antes da alteração |
| NewValues | json | Nullable | Dados depois da alteração |
| IpAddress | string | Não | Endereço IP de origem |

**Enum AuditAction:**

| Valor | Descrição |
|-------|-----------|
| CREATE | Criação de registro |
| UPDATE | Atualização de registro |
| DELETE | Exclusão de registro |
| LOGIN | Login no sistema |
| APPROVE | Aprovação de solicitação |
| REJECT | Rejeição de solicitação |
| DELIVER | Entrega de equipamento |
| RETURN | Devolução de equipamento |

---

## 3. Diagrama de Relacionamentos

```
┌──────────────────┐         ┌──────────────────┐
│    Department    │         │     Location     │
│──────────────────│         │──────────────────│
│ DepartmentId (PK)│         │ LocationId (PK)  │
│ Name             │         │ Name             │
└────────┬─────────┘         │ Address          │
         │                   └────────┬─────────┘
         │ 1:N                        │ 1:N
         ▼                            ▼
┌──────────────────┐         ┌──────────────────┐
│      User        │         │  AssetCategory   │
│──────────────────│         │──────────────────│
│ UserId (PK)      │         │ CategoryId (PK)  │
│ Name             │         │ Name             │
│ Email            │         │ Description      │
│ PasswordHash     │         │ IsActive         │
│ Role             │         └────────┬─────────┘
│ DepartmentId (FK)│                  │ 1:N
│ IsActive         │                  ▼
│ CreatedAt        │         ┌──────────────────┐
└──┬──────┬───┬───┘         │      Asset       │
   │      │   │             │──────────────────│
   │      │   │             │ AssetId (PK)     │
   │      │   │             │ Name             │
   │      │   │             │ PatrimonyNumber  │
   │      │   │             │ SerialNumber     │
   │      │   │             │ AcquisitionDate  │
   │      │   │             │ AcquisitionValue │
   │      │   │             │ Supplier         │
   │      │   │             │ WarrantyUntil    │
   │      │   │             │ Status           │
   │      │   │             │ CategoryId (FK)  │
   │      │   │             │ LocationId (FK)  │
   │      │   │             │ CurrentUserId(FK)│
   │      │   │             │ IsDeleted        │
   │      │   │             └──┬───────────┬───┘
   │      │   │                │           │
   │      │   │                │ 1:N       │ 1:N
   │      │   │                ▼           ▼
   │      │   │   ┌────────────────┐ ┌────────────────┐
   │      │   │   │ AssetMovement  │ │    Request     │
   │      │   │   │────────────────│ │────────────────│
   │      │   │   │ MovementId(PK) │ │ RequestId (PK) │
   │      │   │   │ Type           │ │ Status         │
   │      │   │   │ Date           │ │ Justification  │
   │      │   │   │ AssetId (FK)   │ │ AssetId (FK)   │
   │      │   │   │ FromUserId(FK) │ │ RequestedById  │
   │      │   │   │ ToUserId (FK)  │ │ ApprovedById   │
   │      │   │   │ ResponsibleId  │ │ ApprovedAt     │
   │      │   │   │ Observation    │ │ RejectionReason│
   │      │   │   │ RequestId (FK) │ │ CreatedAt      │
   │      │   │   └────────────────┘ │ UpdatedAt      │
   │      │   │                      └────────────────┘
   │      │   │
   │      │   └──────────────┐
   │      │                  ▼
   │      │         ┌────────────────┐
   │      │         │   AuditLog     │
   │      │         │────────────────│
   │      │         │ LogId (PK)     │
   │      │         │ Action         │
   └──────┼────────▶│ EntityName     │
          │         │ EntityId       │
          │         │ UserId (FK)    │
          │         │ Timestamp      │
          │         │ OldValues      │
          │         │ NewValues      │
          │         │ IpAddress      │
          │         └────────────────┘
          │
          │ Relações User → AssetMovement:
          │   FromUserId (quem devolveu)
          │   ToUserId (quem recebeu)
          │   ResponsibleId (quem executou)
          │
          │ Relações User → Request:
          │   RequestedById (quem pediu)
          │   ApprovedById (quem aprovou)
```

---

## 4. Value Objects

| Value Object | Entidade | Campos | Regras de Validação |
|--------------|----------|--------|---------------------|
| PatrimonyNumber | Asset | Value (string) | Único, não nulo, formato definido pela empresa |
| SerialNumber | Asset | Value (string) | Único, não nulo, formato do fabricante |
| Money | Asset | Amount (decimal), Currency (string) | Amount > 0, Currency = "BRL" |
| Email | User | Value (string) | Formato válido, único no sistema |

---

## 5. Enums

| Enum | Valores |
|------|---------|
| AssetStatus | Available, InUse, InMaintenance, Transferred, Retired |
| MovementType | Delivery, Transfer, Return, Maintenance, Retirement |
| RequestStatus | Pending, Approved, Rejected, Delivered, Returned |
| UserRole | Admin, ITManager, Manager, HR, Facilities |
| AuditAction | CREATE, UPDATE, DELETE, LOGIN, APPROVE, REJECT, DELIVER, RETURN |

---

## 6. Regras de Domínio

| ID | Regra | Entidade |
|----|-------|----------|
| RD-01 | Patrimônio e serial devem ser únicos no sistema | Asset |
| RD-02 | Um ativo não pode ter dois vínculos ativos | Asset |
| RD-03 | Status deve transicionar apenas via ações permitidas | Asset, Request |
| RD-04 | Solicitação só pode ser criada por usuários ativos | Request |
| RD-05 | Aprovação só pode ser feita pelo gestor do departamento | Request |
| RD-06 | Log de auditoria é imutável (sem update/delete) | AuditLog |
| RD-07 | Soft delete em ativos (nunca deletar fisicamente) | Asset |

---

---

## 7. Convenções de Código

### 7.1 Classes Seladas

Todas as entidades, value objects e classes do domínio devem ser declaradas como `sealed`:

```csharp
// ✓ CORRETO
public sealed class Asset { }
public sealed class PatrimonyNumber { }
public sealed class Money { }

// ✗ EVITAR
public class Asset { }
```

**Benefícios:**
- Performance: evita dispatch via vtable
- Segurança: previne herança indesejada
- Imutabilidade: reforça design defensivo

### 7.2 Uso de `var`

Utilizar `var` quando o tipo for óbvio na declaração:

```csharp
// ✓ BOM - tipo óbvio
var asset = new Asset();
var patrimony = new PatrimonyNumber("PAT-001");
var value = new Money(1500.00m, "BRL");

// ✗ EVITAR - tipo não óbvio
Asset asset = GetAssetById(id);
string name = asset.Name;
```

### 7.3 Naming Conventions

| Elemento | Convenção | Exemplo |
|----------|-----------|---------|
| Entidades | `{Noun}` (singular) | `Asset`, `Request`, `User` |
| Value Objects | `{Noun}` (singular) | `PatrimonyNumber`, `Money` |
| Enums | `{Noun}` (singular) | `AssetStatus`, `UserRole` |
| Commands | `{Verb}{Noun}Command` | `CreateAssetCommand` |
| Queries | `{Verb}{Noun}Query` | `GetAssetQuery`, `ListAssetsQuery` |
| Handlers | `{Command/Query}Handler` | `CreateAssetCommandHandler` |

---

## Documentos Relacionados

- `Atlas_ITAM_Requisitos_Funcionais.md` — Requisitos funcionais
- `Atlas_ITAM_Requisitos_Nao_Funcionais.md` — Requisitos não funcionais
- `Atlas_ITAM_Regras_Negocio.md` — Regras de negócio
- `Atlas_ITAM_Arquitetura_Solucao.md` — Arquitetura da solução
- `Atlas_ITAM_Backlog.md` — Backlog do produto
- `Atlas_ITAM_Modelagem_Banco.md` — Modelagem do banco de dados
