# ADR 0004 — Autenticação com JWT

## Status

Aceito

## Contexto

O serviço de pedidos precisa proteger operações de escrita e acesso a dados sensíveis.

## Decisão

Implementar autenticação JWT com token assinado por chave simétrica.

- `POST /api/auth/token` gera o token para credenciais de demonstração.
- Todas as rotas de `/orders` exigem `Authorization: Bearer {token}`.
- O token expira em 2 horas.

## Consequências

- Fluxo simples e stateless.
- Demonstra autenticação moderna para APIs REST.
- Fácil integração com um Identity Provider no futuro.

## Alternativas consideradas

- API Key — não atende o padrão JWT desejado.
- Cookie-based session — não ideal para APIs REST.
- Sem autenticação — inaceitável para rotas de pedido.
