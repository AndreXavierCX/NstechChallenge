# ADR 0001 — Usar PostgreSQL para persistência

## Contexto

O serviço precisa de um banco relacional confiável para armazenar pedidos, itens de pedido e produtos.

## Decisão

Usar **PostgreSQL** como banco de dados principal. O repositório mantém um arquivo `docker-compose.yml` para facilitar a criação de um container PostgreSQL.

## Consequências

- Permite modelar relacionamentos entre `orders`, `order_items` e `products`.
- Suporta índices e tipos numéricos precisos para valores monetários.
- É compatível com EF Core e com o contexto de persistência usado em `src/Nstech.Infrastructure/Persistence/ApplicationDbContext.cs`.
