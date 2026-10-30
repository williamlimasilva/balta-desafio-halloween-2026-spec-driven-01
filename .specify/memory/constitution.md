# Constituição do projeto

## Princípios

### P1. Stack aprovada

- A API e os projetos de teste devem compilar para `net10.0`.
- A aplicação deve expor uma API HTTP usando Minimal APIs do ASP.NET Core.
- A persistência local deve usar SQLite por meio do Entity Framework Core.
- Os testes automatizados devem usar xUnit.

### P2. Limites de arquitetura

- Um handler HTTP não pode acessar `DbContext`, SQL, arquivo de banco ou repositório diretamente; toda operação passa por um serviço de aplicação.
- Regras de geração e validação de senha não podem ser implementadas no handler HTTP.
- O serviço deve depender de abstrações adequadas para persistência e geração, permitindo testar as regras sem iniciar o servidor nem abrir o banco real.

### P3. Regras observáveis de senha

- Toda senha gerada deve ter pelo menos 16 caracteres.
- Toda senha gerada deve conter ao menos um caractere de pontuação permitido.
- Nenhuma senha gerada pode conter caractere de espaço em branco.
- A geração deve usar fonte criptograficamente segura de aleatoriedade.
- Cada regra acima deve possuir teste unitário próprio, além de testes combinados do resultado completo.

### P4. Persistência e finalidade

- Cada geração bem-sucedida deve persistir senha e GUID associados em SQLite antes de confirmar sucesso ao cliente.
- A senha será armazenada em texto puro somente para fins didáticos; a aplicação não pode ser apresentada, publicada ou reutilizada como solução de produção.
- O esquema do banco deve evoluir por migrações versionadas; não substituir migrações por criação automática destrutiva do esquema.

### P5. Contrato HTTP e erros

- Os handlers devem delegar operações ao serviço e expor somente os contratos descritos em `specs/contracts/password-api.md`.
- Identificador inválido deve produzir HTTP 400 com corpo ProblemDetails.
- GUID válido sem senha associada deve produzir HTTP 404.
- Cada endpoint deve ter ao menos um teste de integração de caminho feliz; o caminho de validação inválida também deve ser testado.

### P6. Dependências justificadas

- Cada dependência NuGet adicionada diretamente deve constar em `specs/plan.md` com sua função e justificativa.
- Não adicionar bibliotecas de mocking, validação, geração aleatória ou persistência além das escolhas justificadas no plano.

## Qualidade e governança

- A documentação normativa é `specs/spec.md`; decisões de implementação ficam em `specs/plan.md` e contratos HTTP em `specs/contracts/password-api.md`.
- Uma tarefa em `specs/tasks.md` permanece `[ ]` até que seu critério de conclusão seja verificado e haja evidência registrada.
- A entrega de implementação só é aceita quando build, testes automatizados, migração SQLite e execução local forem verificados sem erros ou warnings.
- Qualquer alteração destes princípios deve ser registrada nesta constituição e refletida nos artefatos afetados antes de implementação.

**Versão**: 1.0.0 | **Ratificada**: 2026-10-30 | **Última alteração**: 2026-10-30
