# ADR 0002 — Clean Architecture

## Status

Aceito

## Contexto

O projeto deve ser organizado em camadas que separem regras de negócio, aplicação e infraestrutura.

## Decisão

Adotar **Clean Architecture** com as seguintes camadas:

1. `Nstech.Domain` — entidades e exceções de domínio.
2. `Nstech.Application` — comandos, queries, handlers e DTOs.
3. `Nstech.Infrastructure` — implementação de persistência EF Core.
4. `Nstech.Api` — composição de dependências e endpoints HTTP.

### Regra de dependência

```
Api → Infrastructure → Application → Domain
```

## Consequências

- Domínio permanece independente de frameworks.
- Casos de uso são testáveis isoladamente.
- Infraestrutura pode ser substituída sem alterar a lógica de negócio.
