# Pesquisa: Gerador de Senhas Fortes

## Escopo

Registrar as decisões técnicas necessárias para o plano, sem ampliar os requisitos de produto. A solução é local e didática; armazenamento em texto puro não é aceitável para uso real.

## Decisões pesquisadas

### SQLite com EF Core

O provider oficial `Microsoft.EntityFrameworkCore.Sqlite` permite usar EF Core com um arquivo SQLite local. SQLite é adequado ao exercício de baixo volume por não exigir servidor de banco. As limitações do provider e do próprio SQLite são aceitas neste escopo.

### Migrações

Migrações versionadas são a estratégia definida para criar e evoluir o esquema preservando dados. A migração inicial ficará no repositório e será aplicada explicitamente com `dotnet ef database update`; não usar `EnsureCreated` como substituto da migração.

### Testes HTTP

A documentação ASP.NET Core 10 recomenda `Microsoft.AspNetCore.Mvc.Testing` para hospedar a aplicação de teste com `WebApplicationFactory`/`TestServer`. O teste pode substituir a conexão configurada e usar SQLite em memória com uma conexão aberta; assim, os testes verificam o provider relacional sem tocar em `data/passwords.db`.

### Aleatoriedade

Usar `System.Security.Cryptography.RandomNumberGenerator`, disponível no runtime, evita acrescentar pacote para geração criptográfica. O algoritmo força pelo menos um caractere de pontuação permitida, preenche o restante do alfabeto explícito e embaralha usando a mesma fonte segura.

## Dependências e justificativas

- `Microsoft.EntityFrameworkCore.Sqlite`: provider de persistência exigido pela escolha SQLite.
- `Microsoft.EntityFrameworkCore.Design` e ferramenta CLI `dotnet-ef`: geração/aplicação de migrações em desenvolvimento; alinhar versão 10.0.x ao EF Core.
- `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`: testes e descoberta com `dotnet test`, conforme stack pedida.
- `Microsoft.AspNetCore.Mvc.Testing`: cliente/servidor in-process para integração dos endpoints.
- Sem biblioteca de mocking ou provider EF InMemory; fakes pequenos para unidade e SQLite em memória para integração evitam abstrações e pacotes desnecessários.

## Referências consultadas

- Microsoft Learn, [SQLite Database Provider - EF Core](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/), consultado em 2026-10-30.
- Microsoft Learn, [Migrations Overview - EF Core](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/), consultado em 2026-10-30.
- Microsoft Learn, [Integration tests in ASP.NET Core 10](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0), consultado em 2026-10-30.

## Questões em aberto

- Nenhuma questão bloqueia a documentação ou o início da implementação planejada. O uso em produção permanece explicitamente proibido pelo escopo.
