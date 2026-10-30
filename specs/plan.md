# Plano técnico

# Plano técnico: Gerador de Senhas Fortes

## Contexto

Este plano implementa `specs/spec.md` sem alterar seus requisitos. A aplicação é um exercício local e didático; senha em texto puro é uma limitação intencional e impede uso em produção. A estrutura prioriza dois endpoints pequenos, serviço de aplicação testável e persistência relacional local.

## Arquitetura

- `src/PasswordGenerator.Api/`: Minimal API, contratos HTTP, serviço, entidade e contexto de persistência.
- Handlers HTTP dependem de `IPasswordService` e traduzem resultados em status e corpos descritos em `specs/contracts/password-api.md`. Não acessam diretamente contexto, repositório ou banco.
- `PasswordService` usa `IPasswordGenerator` e `IPasswordStore`; `EfPasswordStore` implementa persistência por `PasswordDbContext`.
- Regras de geração ficam em `PasswordGenerator`, usando a fonte criptográfica do runtime. Testes exercitam o gerador e o serviço com fakes, sem iniciar o servidor nem abrir o banco real.
- `src/PasswordGenerator.Api/Data/`: entidade `PasswordRecord`, contexto `PasswordDbContext`, `IPasswordStore`/`EfPasswordStore` e migrações versionadas.
- `tests/PasswordGenerator.UnitTests/`: regras de senha isoladas e testes do serviço com store fake.
- `tests/PasswordGenerator.IntegrationTests/`: pipeline HTTP completo com `WebApplicationFactory` e banco SQLite isolado por teste/fixture.
- `PasswordGenerator.sln`: solução que contém API e ambos os projetos de teste.

## Decisões técnicas e justificativas

| Decisão                  | Escolha                                                                                       | Justificativa                                                                                                                                                                                                            |
| ------------------------ | --------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Runtime e linguagem      | .NET 10 / C#; todos os projetos em `net10.0`                                                  | A stack recomendada pelo desafio e o runtime de suporte escolhido para esta entrega.                                                                                                                                     |
| HTTP                     | ASP.NET Core Minimal API                                                                      | O desafio pede API mínima de dois endpoints; handlers enxutos tornam visível a delegação ao serviço.                                                                                                                     |
| Persistência             | SQLite local com EF Core 10                                                                   | Um arquivo local resolve persistência entre reinícios sem servidor externo. `Microsoft.EntityFrameworkCore.Sqlite` é o provider oficial mantido no projeto EF Core.                                                      |
| Evolução do esquema      | Migrações EF Core versionadas                                                                 | A migração inicial documenta/reproduz o esquema e preserva evolução futura sem `EnsureCreated` como mecanismo de produção do esquema. O comando de atualização fica explícito no quickstart.                             |
| Aleatoriedade            | `System.Security.Cryptography.RandomNumberGenerator` da plataforma                            | Geração de senha imprevisível sem dependência externa. Um caractere de pontuação é selecionado obrigatoriamente; os demais são amostrados de um alfabeto explícito e a sequência é embaralhada com a mesma fonte segura. |
| Testes                   | xUnit                                                                                         | Framework solicitado pelo desafio e adequado a testes unitários e de integração.                                                                                                                                         |
| Hospedagem de integração | `Microsoft.AspNetCore.Mvc.Testing`                                                            | Fornece `WebApplicationFactory`/`TestServer` para exercitar requests reais sem abrir porta de rede.                                                                                                                      |
| SQLite nos testes        | SQLite em memória com conexão aberta durante o teste                                          | Preserva semântica do provider relacional e isola dados; o banco de desenvolvimento nunca é usado pelos testes.                                                                                                          |
| Migrações em design time | `Microsoft.EntityFrameworkCore.Design` e ferramenta `dotnet-ef` alinhadas à versão EF Core 10 | Necessárias para gerar e aplicar migrações pela CLI; não são dependências de runtime da API.                                                                                                                             |

Versões de pacotes devem usar a mesma versão estável `10.0.x` entre runtime e pacotes EF Core, e versão compatível 10.0.x para `Microsoft.AspNetCore.Mvc.Testing`. Não fixar números de patch diferentes entre providers, ferramentas e assemblies EF Core.

### Dependências diretas previstas

- `Microsoft.EntityFrameworkCore.Sqlite` (API): provider SQLite para EF Core.
- `Microsoft.EntityFrameworkCore.Design` (API, `PrivateAssets=all`): geração de migrações; ferramenta de desenvolvimento, não runtime.
- `xunit`, `xunit.runner.visualstudio` e `Microsoft.NET.Test.Sdk` (projetos de teste): framework, descoberta pelo runner e integração com `dotnet test`.
- `Microsoft.AspNetCore.Mvc.Testing` (integração): host de teste e cliente HTTP in-process.
- `Microsoft.EntityFrameworkCore.Sqlite` (referência direta também no projeto de integração, se necessário para configurar conexão in-memory): provider usado para garantir comportamento SQLite real em teste.
- Não adicionar Moq/NSubstitute, FluentValidation, provider InMemory, gerador de senha ou outra biblioteca. Fakes pequenos e `RandomNumberGenerator` do runtime cobrem as necessidades sem dependência adicional.

## Geração da senha

- Gerar exatamente 16 caracteres por solicitação; isso satisfaz o mínimo de 16 sem adicionar configuração que não foi pedida.
- Alfabeto permitido: letras ASCII `A-Z` e `a-z`, dígitos `0-9` e pontuação ASCII `!@#$%^&*()-_=+[]{};:,.?/`.
- Selecionar pelo menos um caractere da pontuação, preencher as posições restantes a partir do alfabeto permitido e embaralhar criptograficamente. A lista não contém espaços em branco.
- Não exigir uma quantidade mínima de maiúsculas, minúsculas ou dígitos além do que está na especificação; evitar ampliar regras do produto sem requisito.

## Modelo de dados e banco

- Entidade/tabela: `PasswordRecord` / `PasswordRecords`.
- Chave primária `Id`: GUID não nulo, gerado pela aplicação.
- Campo `Password`: texto não nulo, tamanho 16 na geração atual.
- Não persistir data, usuário, token auxiliar ou metadado não solicitado.
- Caminho padrão explícito: `data/passwords.db`, relativo à raiz do repositório, com pasta criada pela aplicação se ausente. Permitir substituição via `ConnectionStrings:PasswordStore`.
- A primeira migração deve criar `PasswordRecords` e a tabela de histórico de migrações. O caminho de migração é `src/PasswordGenerator.Api/Data/Migrations/`.
- Aplicar o esquema com `dotnet ef database update` antes de executar a API. Não apagar nem recriar o banco automaticamente.
- O banco contém segredos em texto puro: não versionar `data/passwords.db`; a tarefa final de implementação deve ignorar esse arquivo no `.gitignore`.

## Tratamento de erros

- GUID malformado é validação de entrada: HTTP 400 e `application/problem+json` no formato ProblemDetails.
- GUID bem formado sem registro: HTTP 404 e ProblemDetails.
- Falha de persistência não deve resultar em resposta de criação bem-sucedida; a falha é propagada ao tratamento de erro do host como erro de servidor, sem expor detalhes internos.
- O endpoint de geração não recebe parâmetros nem corpo, portanto não há campos de entrada a validar nessa operação.

## Estratégia de testes

- Testes unitários devem cobrir separadamente tamanho mínimo, presença de pontuação, ausência de whitespace e imprevisibilidade/fonte aleatória; também verificar GUID único, armazenamento literal da senha e consulta por meio de fake de repositório.
- O teste de imprevisibilidade deve verificar uso da fonte criptográfica por contrato/injeção, sem afirmar que dois valores aleatórios nunca podem coincidir em toda execução.
- Teste de integração de criação: POST retorna 201, GUID e Location, e registro fica gravado no SQLite de teste.
- Teste de integração de consulta: dado registro persistido, GET retorna 200 e o mesmo texto.
- Teste de integração inválido: identificador malformado retorna 400 ProblemDetails; GUID ausente válido retorna 404.
- Executar `dotnet test PasswordGenerator.sln` e build Release; conferir migration contra um banco de teste limpo e iniciar a API localmente.

## Riscos e limites

- Texto puro deixa cada arquivo de banco, backup e resposta HTTP capaz de expor senha; a advertência deve acompanhar quickstart e contrato.
- SQLite tem limitações de concorrência e tipos em relação a servidores relacionais; aceitável para uso local e baixo volume do desafio.
- Caminho relativo depende da raiz usada para iniciar a aplicação; quickstart padroniza os comandos na raiz e a configuração pode sobrescrever a conexão.
- GUID não é mecanismo de autorização. A consulta não deve ser interpretada como acesso seguro a segredos.

## Referências

- [EF Core SQLite provider](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/)
- [EF Core migrations overview](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [ASP.NET Core integration tests (.NET 10)](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0)
