# Gerador de Senhas Fortes

## Problema

Uma pessoa que precisa de uma senha aleatória com requisitos mínimos não tem uma forma consistente de gerá-la e recuperá-la mais tarde. O sistema deve oferecer geração e consulta por identificador, mantendo explícito que o armazenamento em texto puro é exclusivamente didático.

## Objetivo

Permitir que um cliente solicite uma senha que satisfaça as regras abaixo, receba um identificador único e recupere posteriormente a mesma senha usando esse identificador.

## Usuários

- Pessoa desenvolvedora ou estudante que exercita desenvolvimento orientado por especificações.
- Cliente automatizado que solicita e recupera senhas durante o exercício.

## Histórias

### US1 - Gerar e identificar uma senha (prioridade P1)

Como cliente, quero solicitar uma senha válida e receber um identificador único, para poder recuperá-la posteriormente.

### US2 - Recuperar senha pelo identificador (prioridade P1)

Como cliente, quero consultar uma senha pelo identificador recebido, para obter exatamente a senha gerada anteriormente.

## Requisitos funcionais

- **FR-01**: O sistema deve gerar uma senha quando o cliente solicitar uma nova.
- **FR-02**: O sistema deve associar à senha um identificador único no formato GUID.
- **FR-03**: O sistema deve persistir a senha e seu identificador antes de confirmar uma geração bem-sucedida; o valor persistido será texto puro somente neste exercício didático.
- **FR-04**: O sistema deve permitir recuperar a senha persistida usando seu GUID e retornar o mesmo valor gerado.
- **FR-05**: O sistema deve rejeitar como erro de validação um identificador que não possa ser interpretado como GUID.
- **FR-06**: O sistema deve distinguir identificador inválido de GUID válido sem registro associado.

## Regras de negócio

- **BR-01**: Uma senha gerada deve conter no mínimo 16 caracteres.
- **BR-02**: Uma senha gerada deve conter pelo menos um caractere especial, definido como pontuação visível e não alfanumérica.
- **BR-03**: Uma senha gerada não pode conter espaço em branco, incluindo espaço, tabulação ou quebra de linha.
- **BR-04**: A geração deve ser imprevisível para clientes e não pode depender de uma semente previsível.
- **BR-05**: O GUID deve identificar no máximo uma senha persistida; duas gerações bem-sucedidas devem produzir identificadores distintos.
- **BR-06**: Senhas são persistidas em texto puro apenas para aprendizado. A solução não é adequada para produção.

## Casos de borda

- GUID malformado: erro de validação, sem consulta ao banco.
- GUID bem formado sem registro: resposta de não encontrado.
- GUID existente: retornar a senha exata associada, sem regenerá-la.
- Reinício da aplicação: registros persistidos continuam consultáveis.
- Falha ao persistir uma geração: não confirmar ao cliente uma geração que não foi armazenada.

## Fora de escopo

- Criptografia, hash, cofre de segredos ou qualquer garantia de armazenamento seguro da senha.
- Autenticação, autorização, limitação de requisições, expiração, exclusão ou rotação de senhas.
- Comprimento configurável, composição customizada, interface gráfica ou implantação em nuvem.
- Uso em produção ou tratamento de senhas reais de usuários.

## Critérios de aceite

- **AC-01**: Cada geração satisfaz BR-01, BR-02, BR-03 e BR-04; há testes unitários individuais para cada regra.
- **AC-02**: Cada geração retorna GUID único e deixa a senha persistida para consulta posterior.
- **AC-03**: Consultar GUID existente retorna exatamente a senha anteriormente gerada, inclusive após reinício e reabertura do banco.
- **AC-04**: GUID malformado resulta em erro de validação; GUID válido sem registro resulta em não encontrado.
- **AC-05**: Cada operação pública tem ao menos um teste de integração do caminho feliz.
- **AC-06**: O erro de validação é apresentado ao cliente com status HTTP 400 e corpo ProblemDetails.
- **AC-07**: A documentação e a aplicação informam que a senha é persistida em texto puro e que o projeto é somente didático.
