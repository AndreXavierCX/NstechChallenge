# Requisitos Não Funcionais

## 1. Disponibilidade

O serviço de pedidos deve permanecer disponível para as operações principais:
- criação de pedidos
- confirmação de pedidos
- cancelamento de pedidos
- listagem de pedidos

### Meta

- Disponibilidade esperada: **99,9%**
- Endpoint de health: `GET /health`

## 2. Performance e Escalabilidade

A listagem de pedidos deve suportar paginação e filtros sem impactos de performance significativos.

### Otimizações já aplicadas

- paginação obrigatória (`page`, `pageSize`)
- índices em `CustomerId`, `Status` e `CreatedAt`
- consultas `AsNoTracking()` para leitura

### Metas

- latência p95 para `GET /orders` com filtros: **< 200ms**
- latência p99 para `POST /orders`, `POST /orders/{id}/confirm` e `POST /orders/{id}/cancel`: **< 300ms**

## 3. Segurança

- Autenticação JWT para todas as rotas de pedidos.
- Rate limiting global de **100 requisições por minuto por IP**.
- Credenciais de demonstração: `admin` / `admin123`.

## 4. Resiliência

- Persistência relacional em PostgreSQL.
- Validação de domínio no nível de handlers e entidades.
- A lógica de estoque garante reserva e liberação apropriadas para pedidos confirmados e cancelados.

## 5. Observabilidade

- Health check básico em `GET /health`.
- Swagger disponível na API para documentação interativa.
- Exceções de negócio devolvem payloads estruturados com código e mensagem.

## 6. Limitações atuais

- O repositório contém configurações legadas de RabbitMQ em `appsettings.json` que não estão ativas no fluxo atual.
- O arquivo `docker-compose.yml` faz referência a um serviço RabbitMQ ausente.
- O fluxo atual não usa cache distribuído nem mensageria assíncrona.
