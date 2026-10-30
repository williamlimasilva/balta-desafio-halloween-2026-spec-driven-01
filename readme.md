<img width="100%" alt="Halloween 2026" src="https://baltaio.blob.core.windows.net/static/images/v4/challenges/halloween-2026/banner.jpg" />

## 🎃 Halloween - Desafio 1

Oi, eu sou o William Lima e este é o espaço onde compartilho minha jornada de aprendizado durante o desafio **Halloween 2026**, realizado pelo [balta.io](https://balta.io). 👻

Aqui você vai encontrar projetos, exercícios e códigos que estou desenvolvendo durante o desafio.

### Sobre este desafio

Neste desafio o objetivo é consolidar os fundamentos do Spec Driven Development, criando uma constituição, especificação, planejamento e tarefas manualmente para serem implementadas pela IA posteriormente.

#### O que foi implementado

- API para gerar senhas fortes com caracteres especiais, pelo menos 16 caracteres e sem espaços.
- Cada senha gerada é armazenada em SQLite e retorna um GUID para consulta posterior.
- Persistência com Entity Framework Core e migrações aplicadas na inicialização.
- Testes unitários das regras de geração e testes de integração HTTP.
- Documentação interativa com OpenAPI e Scalar em ambiente de desenvolvimento.

Neste processo eu aprendi:

- ✅ Escrever constituição, especificação, plano e tarefas com regras verificáveis, que guiaram a implementação com IA.
- ✅ Aplicar as migrações na inicialização: um banco novo só funcionou depois de testar a API do zero.
- ✅ Validar a aplicação de ponta a ponta (build, testes, criação, consulta e reinício) antes de publicar.
- ✅ Expor a documentação com OpenAPI e Scalar apenas no ambiente Development.

## Sobre a aplicação

O projeto usa .NET 10, ASP.NET Core Minimal APIs, Entity Framework Core e
SQLite. É um app didático para praticar Spec-Driven Development e não deve ser
usado em produção, pois as senhas são armazenadas sem criptografia.

## Requisitos

- .NET 10 SDK.
- PowerShell.

## Executar localmente

Abra o PowerShell e vá para a raiz do repositório:

```powershell
Set-Location 'C:\Private\Courses\Balta\desafio 1\balta-desafio-halloween-2026-spec-driven-01'
```

Execute os comandos abaixo **a partir da raiz**:

```powershell
# Restaurar as dependências da solução
dotnet restore PasswordGenerator.sln

# Habilitar a documentação interativa (Scalar roda só em Development)
$env:ASPNETCORE_ENVIRONMENT = 'Development'

# Iniciar a API em http://localhost:5101
dotnet run --project src\PasswordGenerator.Api --urls http://localhost:5101
```

As migrações são aplicadas automaticamente ao iniciar. Com a API em execução,
acesse a documentação em [http://localhost:5101/scalar](http://localhost:5101/scalar).

Para encerrar a aplicação, pressione **Ctrl+C** no terminal em que `dotnet run`
está em execução.

### Endpoints

| Método | Rota                  | Descrição                                                                   |
| ------ | --------------------- | --------------------------------------------------------------------------- |
| `POST` | `/api/passwords`      | Gera e armazena uma senha; retorna `201` com o GUID.                        |
| `GET`  | `/api/passwords/{id}` | Retorna a senha associada ao GUID; `400` se inválido, `404` se não existir. |

### Comandos úteis

Execute-os a partir da raiz do repositório:

```powershell
# Compilar a solução
dotnet build PasswordGenerator.sln

# Executar os testes unitários e de integração
dotnet test PasswordGenerator.sln
```

## Estrutura da solução

- `src\PasswordGenerator.Api`: endpoints HTTP, serviços de geração e persistência.
- `tests\PasswordGenerator.UnitTests`: testes unitários das regras de senha.
- `tests\PasswordGenerator.IntegrationTests`: testes de integração da API.
- `specs`: constituição, especificação, plano, tarefas e contrato da API.

O banco SQLite fica em `src\PasswordGenerator.Api\data\passwords.db`, conforme
a connection string `PasswordStore` em `appsettings.json`.

## Bagde

<img src="https://baltaio.blob.core.windows.net/static/images/v4/challenges/halloween-2026/01.png" width="200" />

## Problema

--

## Sobre o Halloween 2026

O desafio **Halloween 2026** consiste em implementar implementar o modelo Spec Driven Development de ponta a ponta, criando apps completas com IA.

### Veja meu progresso no desafio

[Repositório do desafio](https://github.com/williamlimasilva/balta-desafio-halloween-2026-spec-driven-01)
