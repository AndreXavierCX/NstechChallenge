# Guia de Desenvolvimento

Este guia descreve como trabalhar com a implementação atual do Nstech Challenge, cujo foco é um backend de pedidos construído com Clean Architecture e CQRS.

## 1. Pré-requisitos

- .NET SDK 10.0
- PostgreSQL 16 ou equivalente
- Docker Desktop (opcional)
- IDE: Visual Studio, Rider ou VS Code

## 2. Executando localmente

### 2.1 Iniciar PostgreSQL em container

```powershell
docker run -d --name nstech-postgres -e POSTGRES_USER=Nstech -e POSTGRES_PASSWORD=Nstech -e POSTGRES_DB=Nstech -p 5432:5432 postgres:16-alpine
```

### 2.2 Executar a API

```powershell
cd src/Nstech.Api
dotnet run
```

A aplicação será exposta em `http://localhost:8080`.

### 2.3 Nota sobre Docker Compose

O repositório inclui um arquivo `docker-compose.yml` que ainda referencia um serviço RabbitMQ ausente no código atual. Portanto, a forma mais segura de rodar o projeto é usando um container PostgreSQL independente e executando a API diretamente.

## 3. Estrutura do Projeto

- `src/Nstech.Domain/`
  - Regras de negócio do domínio de pedidos
  - Entidades: `Order`, `OrderItem`, `Product`

- `src/Nstech.Application/`
  - Casos de uso via comandos e queries
  - Interfaces de messaging para handlers
  - DTOs e registros de dependências

- `src/Nstech.Infrastructure/`
  - Contexto EF Core (`ApplicationDbContext`)
  - Mapeamentos de tabelas e índices

- `src/Nstech.Api/`
  - Controllers HTTP
  - JWT Authentication
  - Swagger e Health Checks

- `tests/Nstech.UnitTests/`
  - Testes de unidade para os handlers de pedidos

## 4. Fluxos principais

### Criar pedido

1. `POST /api/auth/token` para obter JWT.
2. `POST /orders` com cliente, moeda e itens.
3. `CreateOrderCommandHandler` valida os itens e cria o pedido em estado `Placed`.
4. O pedido é persistido em `orders` e os itens em `order_items`.

### Confirmar pedido

1. `POST /orders/{id}/confirm`.
2. `ConfirmOrderCommandHandler` valida estoque disponível.
3. Produtos com estoque suficiente têm quantidade reservada.
4. O pedido muda para `Confirmed`.

### Cancelar pedido

1. `POST /orders/{id}/cancel`.
2. Se o pedido estava `Confirmed`, o estoque reservado é liberado.
3. O pedido muda para `Canceled`.

### Listar pedidos

1. `GET /orders` com paginação obrigatória (`page`, `pageSize`).
2. Filtragem possível por `customerId`, `status`, `from` e `to`.

## 5. Adicionando um novo endpoint

### Exemplo: `GET /orders/{id}`

1. Criar query e handler em `src/Nstech.Application/Features/Orders/Queries/`.
2. Registrar o handler em `src/Nstech.Application/DependencyInjection.cs`.
3. Adicionar o endpoint em `src/Nstech.Api/Controllers/OrdersController.cs`.

## 6. Rodando os testes

```powershell
dotnet test tests/Nstech.UnitTests/Nstech.UnitTests.csproj
```

## 7. Troubleshooting

- Se a API não conectar ao banco, valide `ConnectionStrings__DefaultConnection`.
- Se o banco não existir, recrie o container ou a base de dados manualmente.
- Não dependa do serviço RabbitMQ no arquivo `docker-compose.yml`, ele não é utilizado na implementação atual.

## 8. Checklist para produção

- substituir credenciais hardcoded e o fluxo demonstrativo de JWT por um Identity Provider.
- remover arquivos e configurações legadas de mensageria se não forem usados.
- adicionar tracing/telemetria e logs estruturados.
- aplicar validação formal de requests e responses.
- revisar política de rate limiting com base no tráfego esperado.
