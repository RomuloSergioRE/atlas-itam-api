# Regras de Negócio — Atlas ITAM

> **Projeto:** Atlas ITAM — Enterprise IT Asset Lifecycle Platform
> **Versão:** 2.0
> **Data:** 2026-07-17
> **Status:** Rascunho
> **Padrão Arquitetural:** CQRS com MediatR

---

## 1. Visão Geral

Este documento descreve as regras de negócio que o Atlas ITAM deve enforce. Diferente dos requisitos funcionais (o que o sistema faz), as regras de negócio definem as **validações, limites e comportamentos** que o sistema deve garantir.

---

## 2. RN-01: Gestão de Ativos

| ID | Regra | Validação |
|----|-------|-----------|
| RN-01.1 | Todo ativo deve ter patrimônio e número de série **únicos** no sistema | Validação no cadastro e edição |
| RN-01.2 | Um ativo não pode ser atribuído a dois usuários ao mesmo tempo | Validação na entrega e transferência |
| RN-01.3 | Ativos com status **Em Manutenção** não podem ser solicitados | Filtro na listagem de disponíveis |
| RN-01.4 | Ativos com status **Baixado** não podem ter movimentações | Validação antes de criar movimentação |
| RN-01.5 | A garantia do ativo não pode ser anterior à data de aquisição | Validação no cadastro e edição |
| RN-01.6 | O valor de aquisição deve ser maior que zero | Validação no cadastro e edição |
| RN-01.7 | Ao cadastrar um ativo, o status padrão deve ser **Disponível** | Definido no CreateAssetCommandHandler |

---

## 3. RN-02: Solicitação e Aprovação

| ID | Regra | Validação |
|----|-------|-----------|
| RN-02.1 | Um colaborador não pode ter mais de **3 solicitações pendentes** simultâneas | Contagem antes de criar solicitação |
| RN-02.2 | Um colaborador não pode solicitar um ativo que já está em seu nome | Verificação de vínculo ativo |
| RN-02.3 | Apenas **Gestores e Admins** podem aprovar solicitações | Validação de perfil na aprovação |
| RN-02.4 | Um gestor só pode aprovar solicitações do **seu time/departamento** | Validação de pertencimento |
| RN-02.5 | Ao aprovar, o sistema deve reservar o ativo (status → Reservado) | Transição automática de estado |
| RN-02.6 | Ao rejeitar, o gestor **deve informar o motivo** | Campo obrigatório no form |
| RN-02.7 | Solicitações pendentes por mais de **30 dias** devem ser notificadas ao admin | Verificação periódica |
| RN-02.8 | Após entrega, o ativo muda para **Em Uso** e vincula ao colaborador | Transição automática de estado |

### 3.1 Fluxo de Estados da Solicitação

```
┌─────────────────────────────────────────────────────────────┐
│                    FLUXO DE SOLICITAÇÃO                      │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  [Colaborador cria] ──▶ PENDENTE                             │
│                              │                               │
│                    ┌─────────┴─────────┐                     │
│                    ▼                   ▼                     │
│              APROVADA            REJEITADA                   │
│                    │             (com motivo)                 │
│                    ▼                                         │
│              ENTREGUE                                        │
│                    │                                         │
│                    ▼                                         │
│              DEVOLVIDA (opcional)                            │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### 3.2 Transições Permitidas

| Estado Atual | Transições Permitidas |
|:---:|:---:|
| Pendente | → Aprovada, Rejeitada |
| Aprovada | → Entregue, Rejeitada (antes da entrega) |
| Entregue | → Devolvida |
| Rejeitada | — (estado final) |
| Devolvida | — (estado final) |

---

## 4. RN-03: Entrega e Devolução

| ID | Regra | Validação |
|----|-------|-----------|
| RN-03.1 | Na entrega, o sistema deve gerar um **termo de responsabilidade em PDF** | Geração automática |
| RN-03.2 | O termo deve conter: dados do ativo, dados do colaborador, data, assinatura | Template obrigatório |
| RN-03.3 | Ao devolver, o colaborador deve informar o **estado do equipamento** | Campo obrigatório |
| RN-03.4 | Equipamentos devolvidos com avarias devem ir para **manutenção** | Transição condicional |
| RN-03.5 | Ao devolver, o ativo volta para **Disponível** e o vínculo é removido | Transição automática |

### 4.1 Estados do Equipamento na Devolução

| Estado | Ação do Sistema |
|--------|----------------|
| Bom estado | Ativo → Disponível |
| Avaria leve | Ativo → Disponível + registro de avaria |
| Avaria grave | Ativo → Em Manutenção |
| Extraviado | Ativo → Baixado (com justificativa) |

---

## 5. RN-04: Transferência

| ID | Regra | Validação |
|----|-------|-----------|
| RN-04.1 | Transferências só podem ser feitas por **Admin ou Gestor TI** | Validação de perfil |
| RN-04.2 | Ao transferir, o ativo sai do usuário atual e vai para o novo | Transição de vínculo |
| RN-04.3 | A transferência deve registrar motivo, data e responsável | Campos obrigatórios |

---

## 6. RN-05: Manutenção

| ID | Regra | Validação |
|----|-------|-----------|
| RN-05.1 | Ao enviar para manutenção, o ativo muda para status **Em Manutenção** | Transição automática |
| RN-05.2 | Ativos em manutenção **não aparecem** como disponíveis | Filtro na listagem |
| RN-05.3 | Ao retornar, o responsável deve informar o que foi feito e se houve custo | Campos obrigatórios |
| RN-05.4 | Se o ativo for considerado irrecuperável, deve ser baixado | Transição condicional |

---

## 7. RN-06: Baixa de Ativo

| ID | Regra | Validação |
|----|-------|-----------|
| RN-06.1 | Apenas **Admin** pode realizar a baixa de um ativo | Validação de perfil |
| RN-06.2 | A baixa é **irreversível** no sistema | Sem reversão permitida |
| RN-06.3 | O motivo da baixa é obrigatório (obsoleto, avariado, extraviado, etc.) | Campo obrigatório |
| RN-06.4 | Ativos com solicitação pendente ou aprovada não podem ser baixados | Verificação de vínculo |
| RN-06.5 | Ao baixar, o ativo sai automaticamente de qualquer vínculo | Limpeza de relacionamentos |

### 7.1 Motivos de Baixa

| Motivo | Descrição |
|--------|-----------|
| Obsoleto | Equipamento sem suporte ou upgrade技术 |
| Avariado | Equipamento com dano irreparável |
| Extraviado | Equipamento desaparecido |
| Roubo/Furto | Equipamento subtraído |
| Doação | Equipamento doado |
| Descarte | Equipamento descartado (lixo eletrônico) |

---

## 8. RN-07: Dashboard e KPIs

| ID | Regra | Validação |
|----|-------|-----------|
| RN-07.1 | Garantias vencendo devem ser alertadas em **30, 60 e 90 dias** | Cálculo automático |
| RN-07.2 | O valor do patrimônio é a soma dos valores de aquisição dos ativos ativos | Query agregada |
| RN-07.3 | Solicitações pendentes devem ser destaque no dashboard dos gestores | Ordenação por prioridade |

---

## 9. RN-08: Auditoria

| ID | Regra | Validação |
|----|-------|-----------|
| RN-08.1 | Toda ação de criar, editar, excluir deve gerar log | Pipeline Behavior automático |
| RN-08.2 | Logs de auditoria **não podem ser editados ou excluídos** | Sem endpoint de delete |
| RN-08.3 | Os logs devem ser mantidos por no mínimo **5 anos** | Política de retenção |
| RN-08.4 | Apenas Admin e Gestor TI podem visualizar logs de auditoria | Validação de perfil |

---

## 10. Resumo

| Módulo | Quantidade de Regras |
|--------|:--------------------:|
| Gestão de Ativos | 7 |
| Solicitação e Aprovação | 8 |
| Entrega e Devolução | 5 |
| Transferência | 3 |
| Manutenção | 4 |
| Baixa de Ativo | 5 |
| Dashboard e KPIs | 3 |
| Auditoria | 4 |
| **Total** | **39** |

---

## Documentos Relacionados

- `Atlas_ITAM_Requisitos_Funcionais.md` — Requisitos funcionais
- `Atlas_ITAM_Requisitos_Nao_Funcionais.md` — Requisitos não funcionais
- `Atlas_ITAM_Escopo_MVP.md` — Escopo do MVP vs evoluções futuras
