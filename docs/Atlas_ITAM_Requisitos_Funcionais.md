# Requisitos Funcionais — Atlas ITAM

> **Projeto:** Atlas ITAM — Enterprise IT Asset Lifecycle Platform
> **Versão:** 2.0
> **Data:** 2026-07-17
> **Status:** Rascunho
> **Padrão Arquitetural:** CQRS com MediatR

---

## 1. Visão Geral

O Atlas ITAM é uma plataforma web para gestão do ciclo de vida de ativos de TI. O documento abaixo descreve todos os requisitos funcionais que o sistema deve atender na versão MVP.

---

## 2. RF-01: Autenticação e Controle de Acesso

**Descrição:** O sistema deve permitir login seguro e controlar acessos por perfil de usuário.

| ID | Requisito | Prioridade |
|----|-----------|------------|
| RF-01.1 | Login com e-mail e senha | Alta |
| RF-01.2 | Autenticação via JWT (access token + refresh token) | Alta |
| RF-01.3 | Controle de acesso por perfil (Admin, Gestor TI, Gestor, RH, Facilities) | Alta |
| RF-01.4 | Logout com invalidação do refresh token | Média |
| RF-01.5 | Recuperação de senha via e-mail | Média |

### 2.1 Perfis e Permissões

| Perfil | Acessa Ativos | Cria Solicitação | Aprova | Dashboard | Gerencia Usuários | Auditoria |
|--------|:---:|:---:|:---:|:---:|:---:|:---:|
| Admin | Sim | Sim | Sim | Sim | Sim | Sim |
| Gestor TI | Sim | Sim | Sim | Sim | Não | Sim |
| Gestor | Sim | Sim | Sim (seu time) | Não | Não | Não |
| RH | Sim (visualizar) | Sim | Não | Não | Não | Não |
| Facilities | Sim (visualizar) | Não | Não | Não | Não | Não |

---

## 3. RF-02: Gestão de Ativos

**Descrição:** CRUD completo de ativos de TI com dados de identificação e rastreabilidade.

| ID | Requisito | Prioridade |
|----|-----------|------------|
| RF-02.1 | Cadastrar novo ativo (nome, categoria, patrimônio, serial, data aquisição, valor, fornecedor, garantia até, localização) | Alta |
| RF-02.2 | Listar ativos com filtros (categoria, status, localização, responsável) | Alta |
| RF-02.3 | Visualizar detalhes completos de um ativo | Alta |
| RF-02.4 | Editar dados do ativo | Alta |
| RF-02.5 | Inativar/remover ativo (soft delete) | Média |
| RF-02.6 | Controle de status: Disponível, Em Uso, Em Manutenção, Transferido, Baixado | Alta |
| RF-02.7 | Busca por número de patrimônio ou número de série | Média |

### 3.1 Campos do Ativo

| Campo | Tipo | Obrigatório | Observação |
|-------|------|:-----------:|------------|
| Nome | string | Sim | Nome descritivo do equipamento |
| Categoria | FK | Sim | Relaciona com AssetCategory |
| Número de Patrimônio | string | Sim | Único no sistema |
| Número de Série | string | Sim | Único no sistema |
| Data de Aquisição | date | Sim | — |
| Valor de Aquisição | decimal | Sim | — |
| Fornecedor | string | Não | — |
| Garantia Até | date | Não | — |
| Localização | FK | Sim | Relaciona com Location |
| Status | enum | Sim | Disponível, Em Uso, Em Manutenção, Transferido, Baixado |
| Responsável | FK | Não | Usuário atual que possui o ativo |
| Observações | text | Não | — |

---

## 4. RF-03: Categorias de Ativos

**Descrição:** Gerenciar as categorias de equipamentos controlados pelo sistema.

| ID | Requisito | Prioridade |
|----|-----------|------------|
| RF-03.1 | Listar categorias disponíveis | Alta |
| RF-03.2 | Criar nova categoria (apenas Admin) | Média |
| RF-03.3 | Editar categoria existente | Média |
| RF-03.4 | Excluir categoria (somente se não houver ativos vinculados) | Baixa |

### 4.1 Categorias Padrão

| Categoria | Descrição |
|-----------|-----------|
| Notebook | Computador portátil |
| Desktop | Computador de mesa |
| Monitor | Tela/monitor |
| Celular | Smartphones corporativos |
| Tablet | Tablets corporativos |
| Dock Station | Estações de acoplamento |
| Impressoras | Impressoras e multifuncionais |
| Servidores | Servidores físicos |
| Switch | Switches de rede |
| Firewall | Dispositivos de segurança |
| Roteadores | Roteadores de rede |
| Licença de Software | Licenças de uso (Microsoft 365, Adobe, etc.) |
| Periféricos | Mouses, teclados, headsets, webcams, etc. |

---

## 5. RF-04: Solicitação de Equipamentos

**Descrição:** Colaboradores solicitam equipamentos que passam por fluxo de aprovação.

| ID | Requisito | Prioridade |
|----|-----------|------------|
| RF-04.1 | Criar solicitação (colaborador seleciona equipamento desejado + justificativa) | Alta |
| RF-04.2 | Status da solicitação: Pendente → Aprovada → Entregue / Rejeitada / Devolvida | Alta |
| RF-04.3 | Gestor recebe notificação da solicitação pendente | Alta |
| RF-04.4 | Gestor aprova ou rejeita com observação | Alta |
| RF-04.5 | Após aprovação, TI registra entrega com termo de responsabilidade em PDF | Alta |
| RF-04.6 | Colaborador pode devolver equipamento (registra devolução) | Média |
| RF-04.7 | Histórico completo da solicitação (quem pediu, quando, quem aprovou, quando entregou) | Alta |

### 5.1 Fluxo de Aprovação

```
┌─────────────┐
│  Colaborador │
│   solicita   │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│   Pendente   │ ← Status inicial
└──────┬──────┘
       │
       ▼
┌─────────────┐     ┌─────────────┐
│   Gestor     │────▶│  Rejeitada  │
│   analisa    │     │  (com obs)  │
└──────┬──────┘     └─────────────┘
       │
       ▼ (Aprovada)
┌─────────────┐
│    TI        │
│   entrega    │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  Entregue    │
└──────┬──────┘
       │
       ▼ (opcional)
┌─────────────┐
│  Devolvida   │
└─────────────┘
```

---

## 6. RF-05: Movimentação de Ativos

**Descrição:** Todo deslocamento ou mudança de status de um ativo deve ser registrado.

| ID | Requisito | Prioridade |
|----|-----------|------------|
| RF-05.1 | Registrar entrega (ativo: Disponível → Em Uso) | Alta |
| RF-05.2 | Registrar transferência entre usuários/departamentos | Média |
| RF-05.3 | Registrar devolução (ativo: Em Uso → Disponível) | Média |
| RF-05.4 | Registrar envio para manutenção | Média |
| RF-05.5 | Registrar retorno da manutenção | Média |
| RF-05.6 | Registrar baixa do ativo | Média |
| RF-05.7 | Histórico completo de movimentações por ativo | Alta |

### 6.1 Campos da Movimentação

| Campo | Tipo | Obrigatório | Observação |
|-------|------|:-----------:|------------|
| Data/Hora | datetime | Sim | Automático |
| Tipo | enum | Sim | Entrega, Transferência, Devolução, Manutenção, Baixa |
| Ativo | FK | Sim | Ativo movimentado |
| Responsável | FK | Sim | Quem realizou a movimentação |
| Usuário Destino | FK | Não | Para quem o ativo foi transferido/entregue |
| Observação | text | Não | Detalhes da movimentação |

---

## 7. RF-06: Controle de Estoque

**Descrição:** Visão consolidada dos ativos disponíveis e em uso.

| ID | Requisito | Prioridade |
|----|-----------|------------|
| RF-06.1 | Quantidade total de ativos por categoria | Alta |
| RF-06.2 | Quantidade disponível vs em uso | Alta |
| RF-06.3 | Alerta de estoque baixo (configurável) | Baixa |

---

## 8. RF-07: Dashboard e Indicadores

**Descrição:** Painel com visão consolidada para gestores.

| ID | Requisito | Prioridade |
|----|-----------|------------|
| RF-07.1 | Total de ativos por status (gráfico de pizza/barras) | Alta |
| RF-07.2 | Ativos por categoria | Alta |
| RF-07.3 | Solicitações pendentes de aprovação | Alta |
| RF-07.4 | Garantias vencendo nos próximos 30/60/90 dias | Média |
| RF-07.5 | Últimas movimentações realizadas | Média |
| RF-07.6 | Valor total do patrimônio controlado | Média |

---

## 9. RF-08: Auditoria

**Descrição:** Registro de todas as ações importantes realizadas no sistema.

| ID | Requisito | Prioridade |
|----|-----------|------------|
| RF-08.1 | Log de criação, edição e exclusão de ativos | Alta |
| RF-08.2 | Log de movimentações realizadas | Alta |
| RF-08.3 | Log de aprovações e rejeições | Alta |
| RF-08.4 | Log de logins dos usuários | Média |
| RF-08.5 | Filtro de auditoria por data, usuário e tipo de ação | Média |

### 9.1 Campos do Log de Auditoria

| Campo | Tipo | Obrigatório | Observação |
|-------|------|:-----------:|------------|
| Data/Hora | datetime | Sim | Automático |
| Usuário | FK | Sim | Quem realizou a ação |
| Ação | enum | Sim | CREATE, UPDATE, DELETE, LOGIN, APPROVE, REJECT, DELIVER, RETURN |
| Entidade | string | Sim | Nome da entidade afetada (Asset, Request, etc.) |
| ID da Entidade | guid | Sim | Identificador do registro afetado |
| Dados Anteriores | json | Não | Snapshot antes da alteração (opcional) |
| Dados Posteriores | json | Não | Snapshot depois da alteração (opcional) |
| IP do Usurio | string | Não | Endereço IP de origem |

---

## 10. Resumo de Prioridades

| Prioridade | Quantidade | Descrição |
|------------|:----------:|-----------|
| Alta | 24 | Funcionalidades essenciais para o MVP funcionar |
| Média | 18 | Funcionalidades importantes mas não bloqueantes |
| Baixa | 3 | Funcionalidades desejáveis para evolução futura |
| **Total** | **45** | — |

---

## Documentos Relacionados

- `Atlas_ITAM_Requisitos_Nao_Funcionais.md` — Requisitos não funcionais
- `Atlas_ITAM_Regras_Negocio.md` — Regras de negócio detalhadas
- `Atlas_ITAM_Escopo_MVP.md` — Escopo do MVP vs evoluções futuras
