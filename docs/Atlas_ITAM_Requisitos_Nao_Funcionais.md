# Requisitos Não Funcionais — Atlas ITAM

> **Projeto:** Atlas ITAM — Enterprise IT Asset Lifecycle Platform
> **Versão:** 2.0
> **Data:** 2026-07-17
> **Status:** Rascunho
> **Padrão Arquitetural:** CQRS com MediatR

---

## 1. Visão Geral

Este documento descreve os requisitos não funcionais do Atlas ITAM — como o sistema deve operar em termos de performance, segurança, escalabilidade, usabilidade e manutenibilidade.

---

## 2. RNF-01: Performance

| ID | Requisito | Meta |
|----|-----------|------|
| RNF-01.1 | Tempo de resposta para listagens | < 2 segundos |
| RNF-01.2 | Tempo de resposta para cadastros/edições | < 1 segundo |
| RNF-01.3 | Tempo de carregamento do dashboard | < 3 segundos |
| RNF-01.4 | Suporte a requisições concorrentes | 50+ usuários simultâneos |
| RNF-01.5 | Paginação obrigatória em listagens | Máx. 20 itens por página |
| RNF-01.6 | Cache para consultas frequentes (dashboard, categorias) | TTL de 30 segundos |

---

## 3. RNF-02: Segurança

| ID | Requisito |
|----|-----------|
| RNF-02.1 | Senhas armazenadas com hash (BCrypt ou Argon2) |
| RNF-02.2 | JWT com validade configurável (sugestão: 15min access, 7d refresh) |
| RNF-02.3 | HTTPS obrigatório em produção |
| RNF-02.4 | Rate limiting para prevenir abuso (login + API) |
| RNF-02.5 | Validação de entrada em todas as APIs (proteção contra SQL Injection, XSS) |
| RNF-02.6 | Logs sem dados sensíveis (senha, token, CPF) |
| RNF-02.7 | CORS configurado apenas para origens do frontend |
| RNF-02.8 | Controle de acesso por perfil (RBAC) em nível de API |

---

## 4. RNF-03: Disponibilidade e Confiabilidade

| ID | Requisito |
|----|-----------|
| RNF-03.1 | Disponibilidade mínima de 99.5% |
| RNF-03.2 | Tratamento global de erros (sem expor stack trace em produção) |
| RNF-03.3 | Backup automático do banco de dados (diário) |
| RNF-03.4 | Retry automático para chamadas externas (se houver) |
| RNF-03.5 | Health check endpoint para monitoramento |

---

## 5. RNF-04: Escalabilidade

| ID | Requisito |
|----|-----------|
| RNF-04.1 | Arquitetura stateless na API (facilita horizontal scaling) |
| RNF-04.2 | Connection pooling no banco de dados |
| RNF-04.3 | Cache para consultas frequentes (InMemory ou Redis) |

---

## 6. RNF-05: Usabilidade

| ID | Requisito |
|----|-----------|
| RNF-05.1 | **Interface 100% responsiva** — desktop, tablet e mobile |
| RNF-05.2 | Design mobile-first com breakpoints: mobile (< 768px), tablet (768px - 1024px), desktop (> 1024px) |
| RNF-05.3 | Navegação intuitiva (máx. 3 cliques para qualquer funcionalidade) |
| RNF-05.4 | Feedback visual em todas as ações (sucesso, erro, loading) |
| RNF-05.5 | Formatação brasileira (moeda R$, data DD/MM/YYYY) |
| RNF-05.6 | Menu lateral colapsável no mobile (hamburger menu) |
| RNF-05.7 | Tabelas com scroll horizontal no mobile |
| RNF-05.8 | Formulários adaptáveis (campos empilhados no mobile) |
| RNF-05.9 | Empty states informativos (quando não há dados) |
| RNF-05.10 | Loading states em todas as operações assíncronas |

---

## 7. RNF-06: Manutenibilidade

| ID | Requisito |
|----|-----------|
| RNF-06.1 | Arquitetura em camadas (Domain, Application, Infrastructure, API) |
| RNF-06.2 | Testes unitários com cobertura mínima de 70% |
| RNF-06.3 | README com instruções de setup e execução |
| RNF-06.4 | Padronização de commits (Conventional Commits) |
| RNF-06.5 | Documentação de APIs via Swagger/OpenAPI |

---

## 8. RNF-07: Portabilidade

| ID | Requisito |
|----|-----------|
| RNF-07.1 | Backend deve rodar em .NET 8+ |
| RNF-07.2 | Frontend deve rodar em Node.js 18+ |
| RNF-07.3 | Banco de dados: PostgreSQL 14+ |
| RNF-07.4 | Dockerizável (docker-compose para ambiente de desenvolvimento) |

---

## 9. RNF-08: Compatibilidade e Responsividade

| ID | Requisito |
|----|-----------|
| RNF-08.1 | Navegadores suportados: Chrome, Firefox, Edge, Safari (últimas 2 versões) |
| RNF-08.2 | **Responsividade obrigatória** em todas as telas do sistema |
| RNF-08.3 | Resolução mínima suportada: 320px (mobile) até 2560px (desktop) |
| RNF-08.4 | Touch-friendly em dispositivos móveis (botões mín. 44px, gestures) |
| RNF-08.5 | Layout adaptativo: sidebar → bottom navigation no mobile |

### 9.1 Breakpoints de Responsividade

| Dispositivo | Largura | Comportamento |
|-------------|---------|---------------|
| Mobile | < 768px | Sidebar vira bottom nav, tabelas com scroll horizontal, formulários empilhados |
| Tablet | 768px - 1024px | Sidebar colapsada, layout de 2 colunas quando possível |
| Desktop | > 1024px | Sidebar completa, layout de múltiplas colunas |

### 9.2 Componentes Responsivos

| Componente | Mobile | Tablet | Desktop |
|------------|--------|--------|---------|
| Sidebar | Bottom navigation | Sidebar colapsada (ícones) | Sidebar completa |
| Tabelas | Scroll horizontal | Colunas reduzidas | Colunas completas |
| Formulários | 1 coluna | 2 colunas | 2-3 colunas |
| Dashboard | Cards empilhados | Grid 2x2 | Grid dinâmico |
| Modais | Fullscreen | Centralizado | Centralizado |
| Botões | Mín. 44px touch | Mín. 40px | Mín. 36px |

---

## 10. Resumo de Prioridades

| Prioridade | Quantidade | Descrição |
|------------|:----------:|-----------|
| Alta | 18 | Requisitos críticos para o funcionamento seguro e responsivo |
| Média | 15 | Requisitos importantes para experiência e manutenção |
| Baixa | 5 | Requisitos desejáveis para evolução |
| **Total** | **38** | — |

---

## Documentos Relacionados

- `Atlas_ITAM_Requisitos_Funcionais.md` — Requisitos funcionais
- `Atlas_ITAM_Regras_Negocio.md` — Regras de negócio detalhadas
- `Atlas_ITAM_Escopo_MVP.md` — Escopo do MVP vs evoluções futuras
