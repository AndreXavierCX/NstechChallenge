# Nstech Challenge — Order Management API

Este repositório contém uma API de gerenciamento de pedidos construída com **.NET 10**, **Clean Architecture**, **CQRS**, **EF Core** e **JWT**.

## Visão Geral

O sistema permite:
- criar pedidos com itens e pedidos;
- confirmar pedidos, reservando estoque;
- cancelar pedidos com liberação de estoque quando aplicável;
- listar pedidos com filtros por cliente, status e período;
- recuperar um token JWT para validar as rotas autenticadas.

## Estrutura do Projeto

- `src/Nstech.Domain/` — entidades de domínio, regras de negócio e exceções de domínio.
- `src/Nstech.Application/` — casos de uso, comandos, queries, DTOs e registro de handlers.
- `src/Nstech.Infrastructure/` — implementação do contexto EF Core e mapeamentos de persistência.
- `src/Nstech.Api/` — ponto de entrada HTTP, controllers, autenticação, filtros e swagger.
- `tests/Nstech.UnitTests/` — testes unitários para handlers de pedido.

## Principais Recursos

- `POST /api/auth/token` — obtém token JWT com credenciais de demonstração.
- `POST /orders` — cria pedido.
- `POST /orders/{id}/confirm` — confirma pedido e reserva estoque.
- `POST /orders/{id}/cancel` — cancela pedido e libera estoque quando necessário.
- `GET /orders` — lista pedidos com paginação e filtros.
- `GET /health` — endpoint de saúde.

## Como Rodar Localmente

### Pré-requisitos

- .NET SDK 10.0
- PostgreSQL 16 ou container Docker PostgreSQL

### Opção A — Com PostgreSQL em container

```powershell
cd c:\repo\Nstech
docker run -d --name nstech-postgres -e POSTGRES_USER=Nstech -e POSTGRES_PASSWORD=Nstech -e POSTGRES_DB=Nstech -p 5432:5432 postgres:16-alpine
```

### Opção B — Com PostgreSQL instalado localmente

Configure a conexão em `src/Nstech.Api/appsettings.json` ou use variáveis de ambiente:

- `ConnectionStrings__DefaultConnection`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`

### Rodar a API

```powershell
cd src/Nstech.Api
dotnet run
```

A API estará disponível em `http://localhost:8080`.

### Verificação

```powershell
curl http://localhost:8080/health
```

## Autenticação

O endpoint `POST /api/auth/token` aceita as credenciais de demonstração:

- `Username`: `admin`
- `Password`: `admin123`

O token retornado deve ser usado no cabeçalho:

```http
Authorization: Bearer {accessToken}
```

## Endpoints da API

### Autenticação

- `POST /api/auth/token`
  - Body: `{ "username": "admin", "password": "admin123" }`
  - Retorna: `{ "accessToken": "..." }`

### Pedidos

- `POST /orders`
  - Body: `{ "customerId": "GUID", "currency": "USD", "items": [{ "productId": "P1", "quantity": 2 }] }`
  - Requer JWT
- `POST /orders/{id}/confirm`
  - Requer JWT
- `POST /orders/{id}/cancel`
  - Requer JWT
- `GET /orders?page=1&pageSize=20&customerId={guid}&status=Placed&from=2026-01-01&to=2026-12-31`
  - Requer JWT

## Testes

Execute os testes com:

```powershell
dotnet test NstechChallenge.sln
```

ou diretamente:

```powershell
dotnet test tests/Nstech.UnitTests/Nstech.UnitTests.csproj
```

## Observações

- A API atual expõe principalmente rotas de pedidos (`/orders`).
- O container `docker-compose.yml` contém referências legadas a RabbitMQ e não está alinhado com a implementação atual.
- O código atual usa `EnsureCreatedAsync()` para criar o banco no startup e `ApplicationDbContext` para persistência.
