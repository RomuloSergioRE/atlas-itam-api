# Modelagem do Banco de Dados — Atlas ITAM

> **Projeto:** Atlas ITAM — Enterprise IT Asset Lifecycle Platform
> **Versão:** 2.0
> **Data:** 2026-07-17
> **Status:** Rascunho
> **Banco:** PostgreSQL 14+
> **Padrão Arquitetural:** CQRS com MediatR (banco único para leitura e escrita)

---

## 1. Diagrama Entidade-Relacionamento (ER)

```
┌─────────────────────┐       ┌─────────────────────┐
│    departments       │       │     locations        │
│─────────────────────│       │─────────────────────│
│ department_id (PK)  │       │ location_id (PK)    │
│ name                │       │ name                │
│ created_at          │       │ address             │
└──────────┬──────────┘       │ created_at          │
           │                  └──────────┬──────────┘
           │ 1:N                         │ 1:N
           ▼                             ▼
┌─────────────────────┐       ┌─────────────────────┐
│      users          │       │  asset_categories    │
│─────────────────────│       │─────────────────────│
│ user_id (PK)        │       │ category_id (PK)    │
│ name                │       │ name                │
│ email (UNIQUE)      │       │ description         │
│ password_hash       │       │ is_active           │
│ role                │       │ created_at          │
│ department_id (FK)  │       └──────────┬──────────┘
│ is_active           │                  │ 1:N
│ created_at          │                  ▼
└──┬──────┬───┬───────┘       ┌─────────────────────┐
   │      │   │               │      assets          │
   │      │   │               │─────────────────────│
   │      │   │               │ asset_id (PK)        │
   │      │   │               │ name                 │
   │      │   │               │ patrimony_number (UQ)│
   │      │   │               │ serial_number (UQ)   │
   │      │   │               │ acquisition_date     │
   │      │   │               │ acquisition_value    │
   │      │   │               │ supplier             │
   │      │   │               │ warranty_until       │
   │      │   │               │ status               │
   │      │   │               │ observations         │
   │      │   │               │ category_id (FK)     │
   │      │   │               │ location_id (FK)     │
   │      │   │               │ current_user_id (FK) │
   │      │   │               │ is_deleted           │
   │      │   │               │ created_at           │
   │      │   │               │ updated_at           │
   │      │   │               └──┬───────────────┬───┘
   │      │   │                  │               │
   │      │   │                  │ 1:N           │ 1:N
   │      │   │                  ▼               ▼
   │      │   │   ┌─────────────────────┐ ┌─────────────────────┐
   │      │   │   │  asset_movements    │ │     requests        │
   │      │   │   │─────────────────────│ │─────────────────────│
   │      │   │   │ movement_id (PK)   │ │ request_id (PK)     │
   │      │   │   │ type               │ │ status              │
   │      │   │   │ date               │ │ justification       │
   │      │   │   │ asset_id (FK)      │ │ asset_id (FK)       │
   │      │   │   │ from_user_id (FK)  │ │ requested_by_id(FK) │
   │      │   │   │ to_user_id (FK)    │ │ approved_by_id (FK) │
   │      │   │   │ responsible_id(FK) │ │ approved_at         │
   │      │   │   │ observation        │ │ rejection_reason    │
   │      │   │   │ request_id (FK)    │ │ created_at          │
   │      │   │   │ created_at         │ │ updated_at          │
   │      │   │   └─────────────────────┘ └─────────────────────┘
   │      │   │
   │      │   └───────────────┐
   │      │                   ▼
   │      │          ┌─────────────────────┐
   │      │          │     audit_logs      │
   │      │          │─────────────────────│
   │      │          │ log_id (PK)         │
   │      └─────────▶│ user_id (FK)        │
   │                 │ action              │
   │                 │ entity_name         │
   │                 │ entity_id           │
   │                 │ old_values (JSONB)  │
   │                 │ new_values (JSONB)  │
   │                 │ ip_address          │
   │                 │ created_at          │
   │                 └─────────────────────┘
   │
   │ Relações via user_id:
   │   asset_movements.from_user_id
   │   asset_movements.to_user_id
   │   asset_movements.responsible_id
   │   requests.requested_by_id
   │   requests.approved_by_id
```

---

## 2. Tabelas

### 2.1 departments

| Coluna | Tipo | Constraints | Descrição |
|--------|------|-------------|-----------|
| department_id | uuid | PK, DEFAULT gen_random_uuid() | Identificador único |
| name | varchar(100) | NOT NULL, UNIQUE | Nome do departamento |
| created_at | timestamp | NOT NULL, DEFAULT now() | Data de criação |

### 2.2 locations

| Coluna | Tipo | Constraints | Descrição |
|--------|------|-------------|-----------|
| location_id | uuid | PK, DEFAULT gen_random_uuid() | Identificador único |
| name | varchar(150) | NOT NULL | Nome da localização |
| address | varchar(255) | NULLABLE | Endereço completo |
| created_at | timestamp | NOT NULL, DEFAULT now() | Data de criação |

### 2.3 asset_categories

| Coluna | Tipo | Constraints | Descrição |
|--------|------|-------------|-----------|
| category_id | uuid | PK, DEFAULT gen_random_uuid() | Identificador único |
| name | varchar(100) | NOT NULL, UNIQUE | Nome da categoria |
| description | varchar(255) | NULLABLE | Descrição |
| is_active | boolean | NOT NULL, DEFAULT true | Se está ativa |
| created_at | timestamp | NOT NULL, DEFAULT now() | Data de criação |

### 2.4 users

| Coluna | Tipo | Constraints | Descrição |
|--------|------|-------------|-----------|
| user_id | uuid | PK, DEFAULT gen_random_uuid() | Identificador único |
| name | varchar(150) | NOT NULL | Nome completo |
| email | varchar(255) | NOT NULL, UNIQUE | E-mail |
| password_hash | varchar(255) | NOT NULL | Hash da senha |
| role | varchar(20) | NOT NULL | Perfil (admin, it_manager, manager, hr, facilities) |
| department_id | uuid | FK → departments | Departamento |
| is_active | boolean | NOT NULL, DEFAULT true | Se está ativo |
| created_at | timestamp | NOT NULL, DEFAULT now() | Data de criação |
| updated_at | timestamp | NOT NULL, DEFAULT now() | Última atualização |

### 2.5 assets

| Coluna | Tipo | Constraints | Descrição |
|--------|------|-------------|-----------|
| asset_id | uuid | PK, DEFAULT gen_random_uuid() | Identificador único |
| name | varchar(200) | NOT NULL | Nome do equipamento |
| patrimony_number | varchar(50) | NOT NULL, UNIQUE | Número de patrimônio |
| serial_number | varchar(100) | NOT NULL, UNIQUE | Número de série |
| acquisition_date | date | NOT NULL | Data de aquisição |
| acquisition_value | decimal(12,2) | NOT NULL, CHECK > 0 | Valor de aquisição |
| supplier | varchar(150) | NULLABLE | Fornecedor |
| warranty_until | date | NULLABLE | Data de término da garantia |
| status | varchar(20) | NOT NULL, DEFAULT 'available' | Status atual |
| observations | text | NULLABLE | Observações |
| category_id | uuid | FK → asset_categories | Categoria |
| location_id | uuid | FK → locations | Localização |
| current_user_id | uuid | FK → users, NULLABLE | Usuário atual |
| is_deleted | boolean | NOT NULL, DEFAULT false | Soft delete |
| created_at | timestamp | NOT NULL, DEFAULT now() | Data de criação |
| updated_at | timestamp | NOT NULL, DEFAULT now() | Última atualização |

### 2.6 requests

| Coluna | Tipo | Constraints | Descrição |
|--------|------|-------------|-----------|
| request_id | uuid | PK, DEFAULT gen_random_uuid() | Identificador único |
| status | varchar(20) | NOT NULL, DEFAULT 'pending' | Status da solicitação |
| justification | text | NOT NULL | Motivo da solicitação |
| asset_id | uuid | FK → assets | Ativo solicitado |
| requested_by_id | uuid | FK → users | Quem solicitou |
| approved_by_id | uuid | FK → users, NULLABLE | Quem aprovou/rejeitou |
| approved_at | timestamp | NULLABLE | Data da aprovação |
| rejection_reason | text | NULLABLE | Motivo da rejeição |
| created_at | timestamp | NOT NULL, DEFAULT now() | Data de criação |
| updated_at | timestamp | NOT NULL, DEFAULT now() | Última atualização |

### 2.7 asset_movements

| Coluna | Tipo | Constraints | Descrição |
|--------|------|-------------|-----------|
| movement_id | uuid | PK, DEFAULT gen_random_uuid() | Identificador único |
| type | varchar(20) | NOT NULL | Tipo (delivery, transfer, return, maintenance, retirement) |
| asset_id | uuid | FK → assets | Ativo movimentado |
| from_user_id | uuid | FK → users, NULLABLE | Usuário de origem |
| to_user_id | uuid | FK → users, NULLABLE | Usuário de destino |
| responsible_id | uuid | FK → users | Quem realizou |
| observation | text | NULLABLE | Detalhes |
| request_id | uuid | FK → requests, NULLABLE | Solicitação vinculada |
| created_at | timestamp | NOT NULL, DEFAULT now() | Data da movimentação |

### 2.8 audit_logs

| Coluna | Tipo | Constraints | Descrição |
|--------|------|-------------|-----------|
| log_id | uuid | PK, DEFAULT gen_random_uuid() | Identificador único |
| user_id | uuid | FK → users | Usuário que executou |
| action | varchar(20) | NOT NULL | Ação realizada |
| entity_name | varchar(50) | NOT NULL | Nome da entidade |
| entity_id | uuid | NOT NULL | ID do registro afetado |
| old_values | jsonb | NULLABLE | Dados antes |
| new_values | jsonb | NULLABLE | Dados depois |
| ip_address | varchar(45) | NULLABLE | IP de origem |
| created_at | timestamp | NOT NULL, DEFAULT now() | Data da ação |

---

## 3. SQL DDL (Data Definition Language)

```sql
-- =====================================================
-- Atlas ITAM — Database Schema
-- PostgreSQL 14+
-- =====================================================

-- Extensão para gerar UUIDs
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- =====================================================
-- TABELA: departments
-- =====================================================
CREATE TABLE departments (
    department_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name          VARCHAR(100) NOT NULL UNIQUE,
    created_at    TIMESTAMP NOT NULL DEFAULT NOW()
);

-- =====================================================
-- TABELA: locations
-- =====================================================
CREATE TABLE locations (
    location_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name        VARCHAR(150) NOT NULL,
    address     VARCHAR(255),
    created_at  TIMESTAMP NOT NULL DEFAULT NOW()
);

-- =====================================================
-- TABELA: asset_categories
-- =====================================================
CREATE TABLE asset_categories (
    category_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name        VARCHAR(100) NOT NULL UNIQUE,
    description VARCHAR(255),
    is_active   BOOLEAN NOT NULL DEFAULT TRUE,
    created_at  TIMESTAMP NOT NULL DEFAULT NOW()
);

-- =====================================================
-- TABELA: users
-- =====================================================
CREATE TABLE users (
    user_id       UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name          VARCHAR(150) NOT NULL,
    email         VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role          VARCHAR(20) NOT NULL CHECK (role IN ('admin', 'it_manager', 'manager', 'hr', 'facilities')),
    department_id UUID NOT NULL REFERENCES departments(department_id),
    is_active     BOOLEAN NOT NULL DEFAULT TRUE,
    created_at    TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_department ON users(department_id);

-- =====================================================
-- TABELA: assets
-- =====================================================
CREATE TABLE assets (
    asset_id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name              VARCHAR(200) NOT NULL,
    patrimony_number  VARCHAR(50) NOT NULL UNIQUE,
    serial_number     VARCHAR(100) NOT NULL UNIQUE,
    acquisition_date  DATE NOT NULL,
    acquisition_value DECIMAL(12,2) NOT NULL CHECK (acquisition_value > 0),
    supplier          VARCHAR(150),
    warranty_until    DATE,
    status            VARCHAR(20) NOT NULL DEFAULT 'available'
                        CHECK (status IN ('available', 'in_use', 'in_maintenance', 'transferred', 'retired')),
    observations      TEXT,
    category_id       UUID NOT NULL REFERENCES asset_categories(category_id),
    location_id       UUID NOT NULL REFERENCES locations(location_id),
    current_user_id   UUID REFERENCES users(user_id),
    is_deleted        BOOLEAN NOT NULL DEFAULT FALSE,
    created_at        TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at        TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_assets_status ON assets(status);
CREATE INDEX idx_assets_category ON assets(category_id);
CREATE INDEX idx_assets_location ON assets(location_id);
CREATE INDEX idx_assets_current_user ON assets(current_user_id);
CREATE INDEX idx_assets_patrimony ON assets(patrimony_number);
CREATE INDEX idx_assets_serial ON assets(serial_number);
CREATE INDEX idx_assets_not_deleted ON assets(is_deleted) WHERE is_deleted = FALSE;

-- =====================================================
-- TABELA: requests
-- =====================================================
CREATE TABLE requests (
    request_id       UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    status           VARCHAR(20) NOT NULL DEFAULT 'pending'
                       CHECK (status IN ('pending', 'approved', 'rejected', 'delivered', 'returned')),
    justification    TEXT NOT NULL,
    asset_id         UUID NOT NULL REFERENCES assets(asset_id),
    requested_by_id  UUID NOT NULL REFERENCES users(user_id),
    approved_by_id   UUID REFERENCES users(user_id),
    approved_at      TIMESTAMP,
    rejection_reason TEXT,
    created_at       TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at       TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_requests_status ON requests(status);
CREATE INDEX idx_requests_requested_by ON requests(requested_by_id);
CREATE INDEX idx_requests_asset ON requests(asset_id);

-- =====================================================
-- TABELA: asset_movements
-- =====================================================
CREATE TABLE asset_movements (
    movement_id    UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    type           VARCHAR(20) NOT NULL CHECK (type IN ('delivery', 'transfer', 'return', 'maintenance', 'retirement')),
    asset_id       UUID NOT NULL REFERENCES assets(asset_id),
    from_user_id   UUID REFERENCES users(user_id),
    to_user_id     UUID REFERENCES users(user_id),
    responsible_id UUID NOT NULL REFERENCES users(user_id),
    observation    TEXT,
    request_id     UUID REFERENCES requests(request_id),
    created_at     TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_movements_asset ON asset_movements(asset_id);
CREATE INDEX idx_movements_from_user ON asset_movements(from_user_id);
CREATE INDEX idx_movements_to_user ON asset_movements(to_user_id);
CREATE INDEX idx_movements_request ON asset_movements(request_id);
CREATE INDEX idx_movements_created ON asset_movements(created_at);

-- =====================================================
-- TABELA: audit_logs
-- =====================================================
CREATE TABLE audit_logs (
    log_id       UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id      UUID NOT NULL REFERENCES users(user_id),
    action       VARCHAR(20) NOT NULL CHECK (action IN ('create', 'update', 'delete', 'login', 'approve', 'reject', 'deliver', 'return')),
    entity_name  VARCHAR(50) NOT NULL,
    entity_id    UUID NOT NULL,
    old_values   JSONB,
    new_values   JSONB,
    ip_address   VARCHAR(45),
    created_at   TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_audit_user ON audit_logs(user_id);
CREATE INDEX idx_audit_entity ON audit_logs(entity_name, entity_id);
CREATE INDEX idx_audit_action ON audit_logs(action);
CREATE INDEX idx_audit_created ON audit_logs(created_at);

-- =====================================================
-- SEED: Categorias padrão
-- =====================================================
INSERT INTO asset_categories (name, description) VALUES
    ('Notebook', 'Computador portátil'),
    ('Desktop', 'Computador de mesa'),
    ('Monitor', 'Tela/monitor'),
    ('Celular', 'Smartphones corporativos'),
    ('Tablet', 'Tablets corporativos'),
    ('Dock Station', 'Estações de acoplamento'),
    ('Impressora', 'Impressoras e multifuncionais'),
    ('Servidor', 'Servidores físicos'),
    ('Switch', 'Switches de rede'),
    ('Firewall', 'Dispositivos de segurança'),
    ('Roteador', 'Roteadores de rede'),
    ('Licença de Software', 'Licenças de uso'),
    ('Periférico', 'Mouses, teclados, headsets, webcams');

-- =====================================================
-- SEED: Departamento padrão
-- =====================================================
INSERT INTO departments (name) VALUES
    ('Tecnologia da Informação'),
    ('Recursos Humanos'),
    ('Financeiro'),
    ('Operações'),
    ('Marketing'),
    ('Administrativo');

-- =====================================================
-- SEED: Localização padrão
-- =====================================================
INSERT INTO locations (name, address) VALUES
    ('Sede SP - Matriz', 'Av. Paulista, 1000 - São Paulo, SP'),
    ('Sede RJ - Filial', 'Rua das Laranjeiras, 200 - Rio de Janeiro, RJ');

-- =====================================================
-- SEED: Usuário Admin padrão (senha: admin123)
-- =====================================================
INSERT INTO users (name, email, password_hash, role, department_id) VALUES
    ('Administrador', 'admin@atlasitam.com',
     '$2a$11$YQ8G3Z6K5X5Y5Y5Y5Y5Y5O5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5',
     'admin',
     (SELECT department_id FROM departments WHERE name = 'Tecnologia da Informação'));
```

---

## 4. Índices

| Tabela | Índice | Coluna(s) | Tipo |
|--------|--------|-----------|------|
| users | idx_users_email | email | UNIQUE |
| users | idx_users_department | department_id | NORMAL |
| assets | idx_assets_status | status | NORMAL |
| assets | idx_assets_category | category_id | NORMAL |
| assets | idx_assets_location | location_id | NORMAL |
| assets | idx_assets_current_user | current_user_id | NORMAL |
| assets | idx_assets_patrimony | patrimony_number | UNIQUE |
| assets | idx_assets_serial | serial_number | UNIQUE |
| assets | idx_assets_not_deleted | is_deleted | PARCIAL (WHERE = FALSE) |
| requests | idx_requests_status | status | NORMAL |
| requests | idx_requests_requested_by | requested_by_id | NORMAL |
| requests | idx_requests_asset | asset_id | NORMAL |
| asset_movements | idx_movements_asset | asset_id | NORMAL |
| asset_movements | idx_movements_from_user | from_user_id | NORMAL |
| asset_movements | idx_movements_to_user | to_user_id | NORMAL |
| asset_movements | idx_movements_request | request_id | NORMAL |
| asset_movements | idx_movements_created | created_at | NORMAL |
| audit_logs | idx_audit_user | user_id | NORMAL |
| audit_logs | idx_audit_entity | entity_name, entity_id | COMPOSTO |
| audit_logs | idx_audit_action | action | NORMAL |
| audit_logs | idx_audit_created | created_at | NORMAL |

---

## 5. Constraints

| Tabela | Constraint | Tipo | Descrição |
|--------|------------|------|-----------|
| users | uk_users_email | UNIQUE | E-mail único |
| users | ck_users_role | CHECK | Roles permitidas |
| assets | uk_assets_patrimony | UNIQUE | Patrimônio único |
| assets | uk_assets_serial | UNIQUE | Serial único |
| assets | ck_assets_status | CHECK | Status permitidos |
| assets | ck_assets_value | CHECK | Valor > 0 |
| requests | ck_requests_status | CHECK | Status permitidos |
| asset_movements | ck_movements_type | CHECK | Tipos permitidos |
| audit_logs | ck_audit_action | CHECK | Ações permitidas |

---

## 6. Relacionamentos

| Tabela Origem | Coluna | Tabela Destino | Tipo |
|---------------|--------|----------------|------|
| users | department_id | departments | N:1 |
| assets | category_id | asset_categories | N:1 |
| assets | location_id | locations | N:1 |
| assets | current_user_id | users | N:1 (nullable) |
| requests | asset_id | assets | N:1 |
| requests | requested_by_id | users | N:1 |
| requests | approved_by_id | users | N:1 (nullable) |
| asset_movements | asset_id | assets | N:1 |
| asset_movements | from_user_id | users | N:1 (nullable) |
| asset_movements | to_user_id | users | N:1 (nullable) |
| asset_movements | responsible_id | users | N:1 |
| asset_movements | request_id | requests | N:1 (nullable) |
| audit_logs | user_id | users | N:1 |

---

## 7. Scripts Úteis

### 7.1 Listar ativos disponíveis

```sql
SELECT a.name, a.patrimony_number, a.serial_number, ac.name as category
FROM assets a
JOIN asset_categories ac ON a.category_id = ac.category_id
WHERE a.status = 'available' AND a.is_deleted = FALSE
ORDER BY ac.name, a.name;
```

### 7.2 Dashboard: ativos por status

```sql
SELECT status, COUNT(*) as total
FROM assets
WHERE is_deleted = FALSE
GROUP BY status
ORDER BY total DESC;
```

### 7.3 Dashboard: garantias vencendo em 90 dias

```sql
SELECT a.name, a.patrimony_number, a.warranty_until,
       u.name as current_user
FROM assets a
LEFT JOIN users u ON a.current_user_id = u.user_id
WHERE a.warranty_until BETWEEN NOW() AND NOW() + INTERVAL '90 days'
  AND a.is_deleted = FALSE
ORDER BY a.warranty_until;
```

### 7.4 Histórico de movimentações de um ativo

```sql
SELECT am.type, am.created_at, 
       u_from.name as from_user,
       u_to.name as to_user,
       u_resp.name as responsible,
       am.observation
FROM asset_movements am
LEFT JOIN users u_from ON am.from_user_id = u_from.user_id
LEFT JOIN users u_to ON am.to_user_id = u_to.user_id
JOIN users u_resp ON am.responsible_id = u_resp.user_id
WHERE am.asset_id = :assetId
ORDER BY am.created_at DESC;
```

### 7.5 Auditoria de um usuário

```sql
SELECT al.action, al.entity_name, al.entity_id,
       al.old_values, al.new_values, al.created_at
FROM audit_logs al
WHERE al.user_id = :userId
ORDER BY al.created_at DESC
LIMIT 100;
```

---

## Documentos Relacionados

- `Atlas_ITAM_Requisitos_Funcionais.md` — Requisitos funcionais
- `Atlas_ITAM_Requisitos_Nao_Funcionais.md` — Requisitos não funcionais
- `Atlas_ITAM_Regras_Negocio.md` — Regras de negócio
- `Atlas_ITAM_Modelagem_Dominio.md` — Modelagem de domínio
- `Atlas_ITAM_Arquitetura_Solucao.md` — Arquitetura da solução
- `Atlas_ITAM_Backlog.md` — Backlog do produto
