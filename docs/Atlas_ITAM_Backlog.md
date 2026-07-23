# Backlog do Produto — Atlas ITAM

> **Projeto:** Atlas ITAM — Enterprise IT Asset Lifecycle Platform
> **Versão:** 2.0
> **Data:** 2026-07-17
> **Status:** Rascunho
> **Padrão Arquitetural:** CQRS com MediatR

---

## 1. Visão Geral

Este documento apresenta o backlog do produto organizado em **épicos**, **histórias de usuário** e **tarefas técnicas**. Cada história segue o formato **"Como [persona], quero [ação], para [benefício]"**.

---

## 2. Épicos

| Épico | ID | Descrição | Prioridade |
|-------|-----|-----------|------------|
| **Infraestrutura CQRS** | **E00** | **Setup MediatR, AutoMapper, Behaviors** | **Alta** |
| Autenticação e Acesso | E01 | Login, JWT, controle de perfis | Alta |
| Gestão de Ativos | E02 | CRUD completo de ativos | Alta |
| Categorias de Ativos | E03 | Gerenciar categorias | Alta |
| Solicitações e Aprovações | E04 | Fluxo de solicitação com aprovação | Alta |
| Movimentação de Ativos | E05 | Histórico completo de movimentações | Alta |
| Controle de Estoque | E06 | Visão consolidada de ativos | Média |
| Dashboard | E07 | KPIs e indicadores | Média |
| Auditoria | E08 | Log de todas as ações | Alta |
| Geração de PDF | E09 | Termo de responsabilidade | Média |
| Frontend | E10 | Interface web responsiva | Alta |

---

## 3. Épico E00: Infraestrutura CQRS

### Histórias de Usuário

| ID | História | Critérios de Aceite | Prioridade |
|----|----------|---------------------|------------|
| H00.01 | Como **desenvolvedor**, quero configurar MediatR no projeto, para usar padrão CQRS | MediatR injetado e funcionando | Alta |
| H00.02 | como **desenvolvedor**, quero criar interfaces base (ICommand, IQuery), para padronizar commands e queries | Interfaces criadas no projeto Application | Alta |
| H00.03 | como **desenvolvedor**, quero configurar AutoMapper, para mapear entidades e DTOs | AutoMapper configurado com profiles | Alta |
| H00.04 | como **desenvolvedor**, quero criar ValidationBehavior, para validar commands antes dos handlers | Pipeline de validação automático | Alta |
| H00.05 | como **desenvolvedor**, quero criar LoggingBehavior, para logar operações | Logs automáticos de commands/queries | Média |

### Tarefas Técnicas

| ID | Tarefa | História | Estimativa |
|----|--------|----------|:----------:|
| T00.01 | Instalar pacotes NuGet (MediatR, AutoMapper, FluentValidation) | H00.01 | 0.5h |
| T00.02 | Configurar MediatR no ServiceCollectionExtensions | H00.01 | 1h |
| T00.03 | Criar interface ICommand\<TResponse\> | H00.02 | 0.5h |
| T00.04 | Criar interface ICommandHandler\<TCommand, TResponse\> | H00.02 | 0.5h |
| T00.05 | Criar interface IQuery\<TResponse\> | H00.02 | 0.5h |
| T00.06 | Criar interface IQueryHandler\<TQuery, TResponse\> | H00.02 | 0.5h |
| T00.07 | Criar record Result\<T\> (IsSuccess, Value, Error) | H00.02 | 1h |
| T00.08 | Configurar AutoMapper no ServiceCollectionExtensions | H00.03 | 0.5h |
| T00.09 | Criar MappingProfile base | H00.03 | 1h |
| T00.10 | Criar ValidationBehavior\<TCommand, TResponse\> | H00.04 | 2h |
| T00.11 | Criar LoggingBehavior\<TCommand, TResponse\> | H00.05 | 1.5h |
| T00.12 | Testes unitários dos Behaviors | H00.04 | 2h |

---

## 4. Épico E01: Autenticação e Controle de Acesso

### Histórias de Usuário

| ID | História | Critérios de Aceite | Prioridade |
|----|----------|---------------------|------------|
| H01.01 | Como **usuário**, quero fazer login com e-mail e senha, para acessar o sistema | JWT retornado; credenciais inválidas retornam 401 | Alta |
| H01.02 | Como **usuário**, quero fazer logout, para encerrar minha sessão | Refresh token invalidado | Média |
| H01.03 | Como **sistema**, quero renovar o access token via refresh token, para manter o usuário autenticado | Novo access token gerado | Alta |
| H01.04 | Como **Admin**, quero criar usuários com perfis diferentes, para controlar quem acessa o sistema | Usuário criado com perfil correto | Alta |
| H01.05 | Como **Admin**, quero listar todos os usuários, para gerenciar o time | Lista retornada com filtros | Média |
| H01.06 | Como **Admin**, quero editar perfil de usuário, para atualizar dados | Dados atualizados | Média |
| H01.07 | Como **Admin**, quero desativar usuário, para remover acesso sem deletar | Usuário marcado como inativo | Média |

### Tarefas Técnicas

| ID | Tarefa | História | Estimativa |
|----|--------|----------|:----------:|
| T01.01 | Criar entidade User no Domain (sealed) | H01.01 | 1h |
| T01.02 | Criar UserRepository e configuração EF Core | H01.01 | 2h |
| T01.03 | Criar LoginCommand + LoginCommandHandler | H01.01 | 2h |
| T01.04 | Criar LoginCommandValidator | H01.01 | 1h |
| T01.05 | Criar RefreshTokenCommand + Handler | H01.03 | 1.5h |
| T01.06 | Criar LogoutCommand + Handler | H01.02 | 1h |
| T01.07 | Criar AuthController (login, refresh, logout) | H01.01 | 1.5h |
| T01.08 | Implementar JwtUserMiddleware | H01.01 | 1h |
| T01.09 | Criar CreateUserCommand + Handler | H01.04 | 1.5h |
| T01.10 | Criar ListUsersQuery + Handler | H01.05 | 1h |
| T01.11 | Criar UpdateUserCommand + Handler | H01.06 | 1h |
| T01.12 | Criar DeactivateUserCommand + Handler | H01.07 | 1h |
| T01.13 | Criar UsersController | H01.04 | 1.5h |
| T01.14 | Seed de dados iniciais (admin default) | H01.01 | 1h |
| T01.15 | Testes unitários dos Handlers de Auth | H01.01 | 2h |

---

## 5. Épico E02: Gestão de Ativos

### Histórias de Usuário

| ID | História | Critérios de Aceite | Prioridade |
|----|----------|---------------------|------------|
| H02.01 | Como **IT Manager**, quero cadastrar um novo ativo, para controlar o patrimônio da empresa | Ativo criado com status "Disponível" | Alta |
| H02.02 | Como **IT Manager**, quero listar ativos com filtros (categoria, status, localização), para encontrar equipamentos rapidamente | Lista paginada com filtros funcionando | Alta |
| H02.03 | Como **IT Manager**, quero visualizar detalhes de um ativo, para ver todas as informações | Todos os campos exibidos | Alta |
| H02.04 | Como **IT Manager**, quero editar dados de um ativo, para manter informações atualizadas | Dados atualizados com validação | Alta |
| H02.05 | Como **Admin**, quero remover (soft delete) um ativo, para desativar sem perder histórico | Ativo marcado como deletado | Média |
| H02.06 | Como **IT Manager**, quero buscar ativo por patrimônio ou serial, para localizar rapidamente | Busca retornando resultado correto | Média |

### Tarefas Técnicas

| ID | Tarefa | História | Estimativa |
|----|--------|----------|:----------:|
| T02.01 | Criar entidade Asset no Domain (sealed) | H02.01 | 2h |
| T02.02 | Criar Value Objects (PatrimonyNumber, SerialNumber, Money) | H02.01 | 2h |
| T02.03 | Criar AssetRepository e configuração EF Core | H02.01 | 2h |
| T02.04 | Criar CreateAssetCommand + Handler | H02.01 | 2h |
| T02.05 | Criar CreateAssetCommandValidator | H02.01 | 1h |
| T02.06 | Criar UpdateAssetCommand + Handler | H02.04 | 1.5h |
| T02.07 | Criar DeleteAssetCommand + Handler (soft delete) | H02.05 | 1h |
| T02.08 | Criar GetAssetQuery + Handler | H02.03 | 1h |
| T02.09 | Criar ListAssetsQuery + Handler (filtros + paginação) | H02.02 | 2h |
| T02.10 | Criar SearchAssetsQuery + Handler | H02.06 | 1h |
| T02.11 | Criar AssetDto + MappingProfile | H02.01 | 1h |
| T02.12 | Criar AssetsController | H02.01 | 1.5h |
| T02.13 | Testes unitários dos Handlers de Asset | H02.01 | 3h |

---

## 6. Épico E03: Categorias de Ativos

### Histórias de Usuário

| ID | História | Critérios de Aceite | Prioridade |
|----|----------|---------------------|------------|
| H03.01 | Como **usuário**, quero listar categorias disponíveis, para selecionar ao criar ativo | Lista retornada | Alta |
| H03.02 | Como **Admin**, quero criar nova categoria, para personalizar o sistema | Categoria criada | Média |
| H03.03 | Como **Admin**, quero editar categoria, para corrigir dados | Dados atualizados | Média |
| H03.04 | Como **Admin**, quero excluir categoria, para remover não utilizadas | Excluída apenas se sem ativos vinculados | Baixa |

### Tarefas Técnicas

| ID | Tarefa | História | Estimativa |
|----|--------|----------|:----------:|
| T03.01 | Criar entidade AssetCategory no Domain (sealed) | H03.01 | 1h |
| T03.02 | Criar AssetCategoryRepository | H03.01 | 1h |
| T03.03 | Criar ListCategoriesQuery + Handler | H03.01 | 1h |
| T03.04 | Criar CreateCategoryCommand + Handler | H03.02 | 1h |
| T03.05 | Criar UpdateCategoryCommand + Handler | H03.03 | 1h |
| T03.06 | Criar CategoriesController | H03.01 | 1h |
| T03.07 | Seed de categorias padrão | H03.01 | 1h |
| T03.08 | Testes unitários | H03.01 | 1h |

---

## 7. Épico E04: Solicitações e Aprovações

### Histórias de Usuário

| ID | História | Critérios de Aceite | Prioridade |
|----|----------|---------------------|------------|
| H04.01 | Como **colaborador**, quero solicitar um equipamento, para ter o que trabalhar | Solicitação criada com status "Pendente" | Alta |
| H04.02 | Como **gestor**, quero ver solicitações pendentes do meu time, para analisar | Lista filtrada por departamento | Alta |
| H04.03 | Como **gestor**, quero aprovar uma solicitação, para liberar o equipamento | Status muda para "Aprovada"; ativo reservado | Alta |
| H04.04 | Como **gestor**, quero rejeitar uma solicitação com motivo, para justificar | Status muda para "Rejeitada" com motivo | Alta |
| H04.05 | Como **IT Manager**, quero registrar entrega de equipamento, para formalizar | Status muda para "Entregue"; PDF gerado | Alta |
| H04.06 | Como **colaborador**, quero devolver equipamento, para quando não preciso mais | Status muda para "Devolvida"; ativo volta para disponível | Média |
| H04.07 | Como **colaborador**, quero ver histórico das minhas solicitações, para acompanhar | Lista de solicitações do usuário | Média |

### Tarefas Técnicas

| ID | Tarefa | História | Estimativa |
|----|--------|----------|:----------:|
| T04.01 | Criar entidade Request no Domain (sealed) | H04.01 | 2h |
| T04.02 | Criar RequestRepository | H04.01 | 2h |
| T04.03 | Criar CreateRequestCommand + Handler | H04.01 | 2h |
| T04.04 | Criar CreateRequestCommandValidator | H04.01 | 1h |
| T04.05 | Criar ApproveRequestCommand + Handler | H04.03 | 1.5h |
| T04.06 | Criar RejectRequestCommand + Handler | H04.04 | 1h |
| T04.07 | Criar DeliverRequestCommand + Handler | H04.05 | 1.5h |
| T04.08 | Criar ReturnRequestCommand + Handler | H04.06 | 1h |
| T04.09 | Criar GetRequestQuery + Handler | H04.07 | 1h |
| T04.10 | Criar ListRequestsQuery + Handler | H04.02 | 1.5h |
| T04.11 | Criar RequestDto + MappingProfile | H04.01 | 1h |
| T04.12 | Criar RequestsController | H04.01 | 1.5h |
| T04.13 | Implementar regra: máx. 3 solicitações pendentes por usuário | H04.01 | 1h |
| T04.14 | Implementar regra: não solicitar ativo já vinculado | H04.01 | 1h |
| T04.15 | Testes unitários dos Handlers de Request | H04.01 | 3h |

---

## 8. Épico E05: Movimentação de Ativos

### Histórias de Usuário

| ID | História | Critérios de Aceite | Prioridade |
|----|----------|---------------------|------------|
| H05.01 | Como **sistema**, quero registrar movimentação a cada mudança de status, para manter histórico | Movimentação criada automaticamente | Alta |
| H05.02 | Como **IT Manager**, quero ver histórico de movimentações de um ativo, para rastrear | Lista de movimentações ordenada por data | Alta |
| H05.03 | Como **IT Manager**, quero registrar transferência entre usuários, para atualizar vínculo | Ativo transferido; movimentação registrada | Média |
| H05.04 | Como **IT Manager**, quero registrar envio para manutenção, para controlar | Ativo vai para "Em Manutenção" | Média |
| H05.05 | Como **IT Manager**, quero registrar retorno da manutenção, para atualizar status | Ativo volta para "Disponível" ou "Baixado" | Média |

### Tarefas Técnicas

| ID | Tarefa | História | Estimativa |
|----|--------|----------|:----------:|
| T05.01 | Criar entidade AssetMovement no Domain (sealed) | H05.01 | 1h |
| T05.02 | Criar AssetMovementRepository | H05.01 | 1h |
| T05.03 | Criar CreateMovementCommand + Handler (usado pelos Handlers de Request) | H05.01 | 2h |
| T05.04 | Criar ListAssetMovementsQuery + Handler | H05.02 | 1h |
| T05.05 | Integrar criação de movimentação nos Handlers de Request | H05.01 | 2h |
| T05.06 | Criar MovementsController | H05.02 | 1h |
| T05.07 | Testes unitários | H05.01 | 2h |

---

## 9. Épico E06: Controle de Estoque

### Histórias de Usuário

| ID | História | Critérios de Aceite | Prioridade |
|----|----------|---------------------|------------|
| H06.01 | Como **gestor**, quero ver quantidade de ativos por categoria, para planejar compras | Dados agregados retornados | Média |
| H06.02 | Como **gestor**, quero ver ativos disponíveis vs em uso, para entender capacidade | Percentuais exibidos | Média |

### Tarefas Técnicas

| ID | Tarefa | História | Estimativa |
|----|--------|----------|:----------:|
| T06.01 | Criar GetStockSummaryQuery + Handler | H06.01 | 2h |
| T06.02 | Criar StockDto + MappingProfile | H06.01 | 1h |
| T06.03 | Criar StockController | H06.01 | 1h |
| T06.04 | Testes | H06.01 | 1h |

---

## 10. Épico E07: Dashboard

### Histórias de Usuário

| ID | História | Critérios de Aceite | Prioridade |
|----|----------|---------------------|------------|
| H07.01 | Como **gestor**, quero ver total de ativos por status, para ter visão geral | Gráfico de pizza/barras exibido | Média |
| H07.02 | Como **gestor**, quero ver ativos por categoria, para identificar composição | Gráfico exibido | Média |
| H07.03 | Como **gestor**, quero ver solicitações pendentes, para priorizar aprovações | Lista destacada | Média |
| H07.04 | Como **gestor**, quero ver garantias vencendo, para planejar substituições | Alertas 30/60/90 dias | Média |
| H07.05 | Como **gestor**, quero ver valor total do patrimônio, para controlar investimento | Valor formatado em R$ | Média |

### Tarefas Técnicas

| ID | Tarefa | História | Estimativa |
|----|--------|----------|:----------:|
| T07.01 | Criar GetDashboardKpisQuery + Handler | H07.01 | 2h |
| T07.02 | Criar GetWarrantyAlertsQuery + Handler | H07.04 | 1.5h |
| T07.03 | Criar DashboardDto + MappingProfile | H07.01 | 1h |
| T07.04 | Criar DashboardController | H07.01 | 1h |
| T07.05 | Testes | H07.01 | 2h |

---

## 11. Épico E08: Auditoria

### Histórias de Usuário

| ID | História | Critérios de Aceite | Prioridade |
|----|----------|---------------------|------------|
| H08.01 | Como **sistema**, quero registrar toda ação importante, para rastreabilidade | Log criado para CRUD, aprovações, login | Alta |
| H08.02 | Como **Admin**, quero listar logs de auditoria com filtros, para investigar | Filtros por data, usuário, ação | Média |
| H08.03 | Como **sistema**, quero que logs sejam imutáveis, para integridade | Sem endpoints de edição/exclusão | Alta |

### Tarefas Técnicas

| ID | Tarefa | História | Estimativa |
|----|--------|----------|:----------:|
| T08.01 | Criar entidade AuditLog no Domain (sealed) | H08.01 | 1h |
| T08.02 | Criar AuditRepository | H08.01 | 1h |
| T08.03 | Criar ListAuditLogsQuery + Handler | H08.02 | 1.5h |
| T08.04 | Criar AuditBehavior (MediatR Pipeline) para logs automáticos | H08.01 | 3h |
| T08.05 | Criar AuditController (somente leitura) | H08.02 | 1h |
| T08.06 | Testes | H08.01 | 2h |

---

## 12. Épico E09: Geração de PDF

### Histórias de Usuário

| ID | História | Critérios de Aceite | Prioridade |
|----|----------|---------------------|------------|
| H09.01 | Como **IT Manager**, quero gerar termo de responsabilidade em PDF, para formalizar entrega | PDF com dados do ativo e colaborador | Média |
| H09.02 | Como **IT Manager**, quero baixar o PDF gerado, para arquivar | Download funcionando | Média |

### Tarefas Técnicas

| ID | Tarefa | História | Estimativa |
|----|--------|----------|:----------:|
| T09.01 | Configurar QuestPDF no projeto | H09.01 | 1h |
| T09.02 | Criar PdfService com template do termo | H09.01 | 3h |
| T09.03 | Integrar geração de PDF no DeliverRequestCommandHandler | H09.01 | 1h |
| T09.04 | Criar endpoint de download | H09.02 | 1h |
| T09.05 | Testes | H09.01 | 1h |

---

## 13. Épico E10: Frontend

### Histórias de Usuário

| ID | História | Critérios de Aceite | Prioridade |
|----|----------|---------------------|------------|
| H10.01 | Como **usuário**, quero fazer login pela interface, para acessar o sistema | Tela de login funcional | Alta |
| H10.02 | Como **gestor**, quero ver o dashboard, para ter visão geral | KPIs exibidos com gráficos | Alta |
| H10.03 | Como **usuário**, quero listar e buscar ativos, para encontrar equipamentos | Tabela com filtros e busca | Alta |
| H10.04 | Como **IT Manager**, quero criar/editar ativo, para gerenciar patrimônio | Formulário funcional | Alta |
| H10.05 | Como **colaborador**, quero criar solicitação, para pedir equipamento | Formulário funcional | Alta |
| H10.06 | Como **gestor**, quero aprovar/rejeitar solicitações, para gerenciar demandas | Ações de aprovação funcionais | Alta |
| H10.07 | Como **usuário**, quero ver detalhes de um ativo, para ver informações completas | Página de detalhes | Média |
| H10.08 | Como **Admin**, quero gerenciar usuários, para controlar acessos | CRUD de usuários | Média |
| H10.09 | Como **Admin**, quero ver logs de auditoria, para investigar ações | Tabela de auditoria | Média |
| H10.10 | Como **usuário**, quero interface responsiva, para usar em mobile e desktop | Layout adaptativo | Alta |

### Tarefas Técnicas

| ID | Tarefa | História | Estimativa |
|----|--------|----------|:----------:|
| T10.01 | Scaffold do projeto Next.js | H10.01 | 1h |
| T10.02 | Configurar Styled Components + theme | H10.01 | 2h |
| T10.03 | Criar componentes base (Sidebar, Header, Table, Card, Modal) | H10.01 | 4h |
| T10.04 | Implementar service de API (Axios + interceptores) | H10.01 | 2h |
| T10.05 | Criar hooks (useAuth, useAssets, useRequests) | H10.01 | 3h |
| T10.06 | Tela de Login | H10.01 | 2h |
| T10.07 | Tela de Dashboard | H10.02 | 3h |
| T10.08 | Tela de Listagem de Ativos | H10.03 | 3h |
| T10.09 | Tela de Formulário de Ativo | H10.04 | 3h |
| T10.10 | Tela de Detalhes do Ativo | H10.07 | 2h |
| T10.11 | Tela de Solicitações | H10.05 | 3h |
| T10.12 | Tela de Aprovações | H10.06 | 2h |
| T10.13 | Tela de Usuários | H10.08 | 2h |
| T10.14 | Tela de Auditoria | H10.09 | 2h |
| T10.15 | Responsividade mobile (breakpoints) | H10.10 | 3h |
| T10.16 | Proteção de rotas (auth guard) | H10.01 | 1h |

---

## 14. Resumo do Backlog

| Épico | Histórias | Tarefas | Estimativa Total |
|-------|:---------:|:-------:|:----------------:|
| **E00: Infraestrutura CQRS** | **5** | **12** | **~12h** |
| E01: Autenticação | 7 | 15 | ~20h |
| E02: Gestão de Ativos | 6 | 13 | ~22h |
| E03: Categorias | 4 | 8 | ~8h |
| E04: Solicitações | 7 | 15 | ~21h |
| E05: Movimentações | 5 | 7 | ~11h |
| E06: Estoque | 2 | 4 | ~5h |
| E07: Dashboard | 5 | 5 | ~8h |
| E08: Auditoria | 3 | 6 | ~11h |
| E09: PDF | 2 | 5 | ~7h |
| E10: Frontend | 10 | 16 | ~37h |
| **Total** | **56** | **106** | **~162h** |

---

## 15. Ordem de Execução Sugerida

```
Fase 0 (Sprint 0): Fundação CQRS
└── E00: Infraestrutura CQRS (MediatR, AutoMapper, Behaviors)

Fase 1 (Sprint 1): Core Backend
├── E01: Autenticação
├── E02: Gestão de Ativos
└── E03: Categorias

Fase 2 (Sprint 2): Core Business
├── E04: Solicitações
└── E05: Movimentações

Fase 3 (Sprint 3): Visibilidade
├── E06: Estoque
├── E07: Dashboard
├── E08: Auditoria
└── E09: PDF

Fase 4 (Sprint 4): Frontend
└── E10: Frontend (todas as telas)
```

---

## 16. Convenções de Código

### Classes Seladas
Todas as classes devem ser declaradas como `sealed`:
- Entities, Value Objects, DTOs, Commands, Queries, Handlers
- Exceção: Validators (herdam de AbstractValidator)

### Uso de `var`
Utilizar `var` quando o tipo for óbvio:
- `var asset = new Asset();` ✓
- `var command = new CreateAssetCommand();` ✓
- `Asset asset = GetAssetById(id);` ✗

### Naming Commands/Queries
- Commands: `{Verb}{Noun}Command` (ex: `CreateAssetCommand`)
- Queries: `{Verb}{Noun}Query` (ex: `GetAssetQuery`, `ListAssetsQuery`)

---

## Documentos Relacionados

- `Atlas_ITAM_Requisitos_Funcionais.md` — Requisitos funcionais
- `Atlas_ITAM_Requisitos_Nao_Funcionais.md` — Requisitos não funcionais
- `Atlas_ITAM_Regras_Negocio.md` — Regras de negócio
- `Atlas_ITAM_Modelagem_Dominio.md` — Modelagem de domínio
- `Atlas_ITAM_Arquitetura_Solucao.md` — Arquitetura da solução
- `Atlas_ITAM_Modelagem_Banco.md` — Modelagem do banco de dados