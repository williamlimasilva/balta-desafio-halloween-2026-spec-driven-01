# Contrato HTTP: Gerador de Senhas Fortes

Base path: `/api/passwords`. JSON usa `application/json`, nomes de propriedade em camelCase e GUID no formato textual padrão (`D`). A API não requer autenticação; isso é apenas um exercício local e didático.

## Criar senha

### `POST /api/passwords`

- Request body: vazio; não há campos ou opções fornecidos pelo cliente.
- Sucesso: `201 Created`, `Content-Type: application/json`, cabeçalho `Location: /api/passwords/{id}`.
- O registro deve ter sido persistido antes da resposta de sucesso.
- Corpo:

```json
{
  "id": "6f9619ff-8b86-d011-b42d-00cf4fc964ff"
}
```

- O valor da senha é obtido pelo endpoint de consulta, não pelo corpo de criação.
- Falha de persistência não pode retornar `201`.

## Consultar senha

### `GET /api/passwords/{id}`

- Parâmetro `id`: GUID textual obrigatório.
- GUID válido encontrado: `200 OK`, `Content-Type: application/json`.
- Corpo:

```json
{
  "id": "6f9619ff-8b86-d011-b42d-00cf4fc964ff",
  "password": "exemplo-nao-utilizar-como-segredo"
}
```

- O campo `password` é o mesmo valor persistido; não regenerar durante a consulta.
- `id` malformado: `400 Bad Request`, `Content-Type: application/problem+json`, corpo ProblemDetails. Não consultar o banco.
- GUID válido sem registro: `404 Not Found`, `Content-Type: application/problem+json`, corpo ProblemDetails.

## ProblemDetails

Os erros seguem o objeto ProblemDetails com `type`, `title`, `status`, `detail` e `instance`. Para erro 400, incluir também `errors.id` como lista de mensagens de validação. Os detalhes não devem conter stack trace, connection string nem outros dados internos.

Exemplo de GUID inválido:

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Validation failed",
  "status": 400,
  "detail": "The id must be a valid GUID.",
  "instance": "/api/passwords/not-a-guid",
  "errors": {
    "id": ["The id must be a valid GUID."]
  }
}
```

## Regras transversais

- A geração produz uma senha de 16 caracteres para satisfazer o mínimo definido; deve conter pontuação permitida e nenhum whitespace.
- Cada sucesso de criação recebe GUID único e registro persistido em SQLite.
- Senha em texto puro é intencional apenas para aprendizado. Não usar em produção.
