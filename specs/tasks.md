# Tarefas

# Tarefas: Gerador de Senhas Fortes

**Documentos de entrada**: `specs/spec.md`, `specs/plan.md`, `specs/research.md`, `specs/data-model.md` e `specs/contracts/password-api.md`.

**Regra de estado**: todas as tarefas começam e permanecem `[ ]` até execução e verificação de seus critérios. Nenhuma implementação foi feita nesta fase documental.

## Fase 1: Estrutura

- [x] T001 [P] Criar `PasswordGenerator.sln`, API e projetos `PasswordGenerator.UnitTests` e `PasswordGenerator.IntegrationTests`, todos em `net10.0`, com os pacotes justificados em `specs/plan.md`. Requisito: constituição P1. Depende de: nenhum. Concluída quando: restore e build da solução passam.

## Fase 2: Persistência

- [x] T002 Criar `PasswordRecord`, `PasswordDbContext`, `IPasswordStore`/`EfPasswordStore`, configuração SQLite `data/passwords.db` e migração inicial em `src/PasswordGenerator.Api/Data/Migrations/`. Requisitos: FR-02, FR-03, BR-05, AC-02, AC-03. Depende de: T001. Concluída quando: `dotnet ef database update` aplica a migração em banco vazio e a tabela contém GUID e senha.

## Fase 3: Serviço de geração (US1, P1)

- [x] T003 Implementar `IPasswordGenerator`, gerador seguro, `IPasswordService` e serviço de aplicação em `src/PasswordGenerator.Api/Services/`; registrar as implementações no contêiner da aplicação. Requisitos: FR-01 a FR-03, BR-01 a BR-06, AC-01 e AC-02. Depende de: T002. Concluída quando: criação persiste antes de retornar, consulta usa o store e a aplicação compila.
- [x] T004 Criar testes unitários em `tests/PasswordGenerator.UnitTests/` para BR-01 (mínimo 16), BR-02 (pontuação), BR-03 (nenhum whitespace), BR-04 (fonte imprevisível), BR-05 (GUID distinto) e BR-06 (texto persistido sem transformação), além da delegação do serviço. Requisitos: BR-01 a BR-06, AC-01 e AC-02. Depende de: T003. Concluída quando: cada regra tem teste identificável e a suíte unitária passa sem usar o banco local.

## Fase 4: História US2 - Consultar senha por GUID (P1)

**Teste independente**: requests HTTP contra host de teste e SQLite isolado validam criação, consulta e erros sem depender da ordem entre casos.

- [x] T005 Criar teste de integração do POST em `tests/PasswordGenerator.IntegrationTests/`, verificando `201`, GUID, `Location` e registro persistido em SQLite de teste. Requisitos: FR-01 a FR-03, AC-02 e AC-05. Depende de: T001, T002. Concluída quando: o teste verifica todo o contrato de sucesso da criação.
- [x] T006 Criar teste de integração do GET em `tests/PasswordGenerator.IntegrationTests/`, preparando um registro e verificando `200` e a senha literal associada. Requisitos: FR-04, AC-03 e AC-05. Depende de: T001, T002. Concluída quando: o teste comprova consulta sem regeneração.
- [x] T007 Criar testes de integração de identificador malformado e GUID válido inexistente em `tests/PasswordGenerator.IntegrationTests/`. Requisitos: FR-05, FR-06, AC-04 e AC-06. Depende de: T001, T002. Concluída quando: o primeiro caso verifica `400` ProblemDetails e o segundo verifica `404` ProblemDetails.
- [x] T008 Mapear `POST /api/passwords` e `GET /api/passwords/{id}` em `src/PasswordGenerator.Api/`, delegando todas as operações ao serviço e seguindo `specs/contracts/password-api.md`. Requisitos: FR-01 a FR-06, AC-02 a AC-06. Depende de: T003, T005, T006, T007. Concluída quando: handlers não acessam persistência e todos os testes de integração passam.

## Fase 5: Verificação final

- [ ] T009 Atualizar `.gitignore` para não versionar `data/passwords.db`; executar os comandos de `specs/quickstart.md`, build, todos os testes, migração em banco limpo e smoke test local. Requisitos: BR-06, AC-03, AC-07 e constituição P4. Depende de: T008. Concluída quando: build sem erros/warnings, testes passam, API inicia, e o GUID de criação continua recuperando a mesma senha após reiniciar a API com o mesmo banco; registrar evidências sem incluir senha nos logs.

## Dependências e ordem

- T001 é a fundação; T002 depende dos projetos criados.
- T003 depende do modelo/store de T002; T004 valida suas regras com testes unitários.
- T005, T006 e T007 podem ser escritos em paralelo depois de T001/T002, usando SQLite em memória; T008 integra os handlers após o serviço e os testes.
- T009 só começa após os dois endpoints funcionarem e verifica a entrega ponta a ponta.
- Todos os itens permanecem não concluídos até que seus critérios sejam realmente executados e verificados.
