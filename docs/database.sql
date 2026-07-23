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
