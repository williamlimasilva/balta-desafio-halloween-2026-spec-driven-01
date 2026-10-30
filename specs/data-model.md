# Modelo de dados: Gerador de Senhas Fortes

## Entidade `PasswordRecord`

| Campo      | Tipo lógico | Restrições                                         | Uso                                              |
| ---------- | ----------- | -------------------------------------------------- | ------------------------------------------------ |
| `Id`       | GUID        | Obrigatório; chave primária; gerado pela aplicação | Identifica e localiza uma geração.               |
| `Password` | Texto       | Obrigatório; contém o valor em texto puro          | Valor imutável gerado e recuperado pelo serviço. |

## Persistência

- Tabela: `PasswordRecords`.
- Um registro representa uma senha gerada; não há relação com usuário ou outros registros.
- Cada criação persiste exatamente um registro antes de responder sucesso.
- A senha gerada atualmente terá 16 caracteres, embora a regra de produto seja mínimo de 16.
- GUID é único pela chave primária. Não há índice adicional.
- Banco padrão: arquivo SQLite `data/passwords.db`, com caminho configurável por `ConnectionStrings:PasswordStore`.
- Migração versionada: `src/PasswordGenerator.Api/Data/Migrations/InitialCreate` (nome da migração inicial).
- Migração deve criar a tabela e sua chave. Regras de conteúdo da senha são verificadas no serviço e nos testes, não por constraint SQL não portável.

## Dados que não serão armazenados

- Data de criação, identidade do solicitante, senha original, hash, salt, token de acesso, expiração ou histórico.
- Nenhum dado além do GUID e da senha é necessário para os dois fluxos pedidos.

## Sensibilidade

`Password` é segredo em texto puro. O banco, backups e resposta de consulta devem ser tratados como dados sensíveis; o arquivo não deve ser versionado. Este modelo só é permitido no exercício de aprendizado e não satisfaz requisitos de armazenamento de credenciais em produção.
