# Quickstart: Gerador de Senhas Fortes

> Exercício didático. O banco armazena senhas em texto puro. Não use com senhas reais nem em produção.

## Pré-requisitos

- Windows, macOS ou Linux.
- .NET SDK 10 instalado.
- Ferramenta `dotnet-ef` na versão 10.0.x alinhada aos pacotes Entity Framework Core do projeto.

## Restaurar e criar banco

Execute os comandos na raiz do repositório, depois que as tarefas de implementação estiverem concluídas:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.0
dotnet restore PasswordGenerator.sln
dotnet ef database update --project src/PasswordGenerator.Api/PasswordGenerator.Api.csproj --startup-project src/PasswordGenerator.Api/PasswordGenerator.Api.csproj
```

Se uma versão 10.0.x mais recente estiver escolhida nos pacotes do projeto, instale a mesma versão da ferramenta `dotnet-ef` em vez de `10.0.0`.

O banco persistente padrão é `data/passwords.db`, relativo à raiz do repositório. A pasta será criada se não existir. Uma connection string configurada em `ConnectionStrings:PasswordStore` substitui o padrão. O arquivo contém senhas em texto puro e deve permanecer fora do controle de versão.

## Executar testes

```powershell
dotnet test PasswordGenerator.sln
```

Os testes de integração usam SQLite em memória e não leem nem modificam o banco persistente local.

## Executar a API

```powershell
dotnet run --project src/PasswordGenerator.Api/PasswordGenerator.Api.csproj
```

## Fluxo HTTP

1. Envie `POST /api/passwords` sem corpo para gerar e persistir uma senha. A resposta contém GUID e cabeçalho `Location`.
2. Envie `GET /api/passwords/{id}` usando o GUID retornado para recuperar o valor persistido.
3. Um identificador malformado resulta em HTTP 400 ProblemDetails; um GUID válido sem registro resulta em HTTP 404 ProblemDetails.

Use `specs/contracts/password-api.md` para corpos e cabeçalhos completos. Não copie senhas para logs, issues, histórico de shell ou commits.
