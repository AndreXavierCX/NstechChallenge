# ADR 0003 — CQRS (Command Query Responsibility Segregation)

## Status

Aceito

## Contexto

O serviço de pedidos executa operações de escrita e leitura com responsabilidades diferentes.

## Decisão

Aplicar **CQRS** para separar:

- comandos que alteram o estado do sistema (`CreateOrder`, `ConfirmOrder`, `CancelOrder`)
- queries que retornam dados de leitura (`GetOrders`)

Cada operação é tratada por um handler específico que implementa interfaces `ICommandHandler` ou `IQueryHandler`.

## Consequências

- Responsabilidades claras para cada caso de uso.
- Melhor testabilidade dos handlers.
- Leitura otimizada sem misturar lógica de escrita.

## Alternativas consideradas

- arquitetura monolítica com repositório único — rejeitada por misturar leitura e escrita.
- MediatR — desnecessário para o escopo atual.
- Event Sourcing — complexidade excessiva para esse problema.
