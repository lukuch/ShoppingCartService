# ShoppingCart Service

A .NET 8 backend service that integrates with the Platzi Fake Store API to let users:

- Browse available products
- Add products to a shopping cart
- View cart contents
- View a summary of the current order

## Tech Stack

- ASP.NET Core Minimal APIs with Carter
- MediatR (CQRS)
- Marten (PostgreSQL document storage)
- Redis (caching)
- Refit + Polly (external API client with resiliency)
- Serilog (structured logging)
- Swagger/OpenAPI

## Architecture

- Domain-Driven Design (DDD): Business logic centers on the `Cart` aggregate (and `CartItem`) in `ShoppingCart.Domain`, enforced through explicit invariants and domain events.
- Clean Architecture: Clear boundaries (`API` → `Application` → `Domain` → `Infrastructure`) with dependencies pointing inward; the `Application` layer defines ports like `IProductService` and `ICartRepository`.
- Event Sourcing (Marten): The `Cart` emits events (`ItemAdded`, `ItemQuantityUpdated`, `ItemRemoved`) and applies them via `Apply(...)`. Marten persists event streams and rehydrates aggregate state.

## Getting Started

### Prerequisites

- Docker and Docker Compose
- Alternatively, for local runs without Docker: .NET 8 SDK, PostgreSQL, and Redis

### Run with Docker Compose (recommended)

```bash
docker compose up --build
```

Services started:
- API at http://localhost:8080
- Swagger UI at http://localhost:8080/swagger
- Health checks at http://localhost:8080/health
- PostgreSQL at localhost:55432
- Redis at localhost:56379

The compose file sets environment variables for the API, including database, cache, and external Product API base URL.

### Run locally without Docker

1) Start PostgreSQL and Redis locally and note their connection strings.

2) From the repo root, run the API:

```bash
cd src/ShoppingCart.API
dotnet run
```

3) Configure using `src/ShoppingCart.API/appsettings.Development.json` or environment variables:

- `ConnectionStrings__PostgreSQL` (e.g. `Host=localhost;Port=55432;Database=shoppingcart;Username=postgres;Password=postgres123`)
- `ConnectionStrings__Redis` (e.g. `localhost:56379`)
- `ProductApi__BaseUrl` (defaults to `https://api.escuelajs.co`)

### API Reference

- GET `/api/products` — list products
- GET `/api/products/{id}` — get product details
- POST `/api/cart/{userId}/items` — add item to cart
  - body: `{ "productId": number, "quantity": number }`
- PATCH `/api/cart/{userId}/items/{productId}` — update item quantity
  - body: `{ "quantity": number }`
- DELETE `/api/cart/{userId}/items/{productId}` — remove item
- GET `/api/cart/{userId}` — get cart
- GET `/api/cart/{userId}/summary` — get cart summary (subtotal, tax, total, item count)

See `ShoppingCart.postman_collection.json` for a runnable flow.

## Configuration

- External Product API is configured via `ProductApi:BaseUrl` (or `ProductApi__BaseUrl`).
- Default base URL: `https://api.escuelajs.co` (current host for Platzi Fake Store API).
- Redis caching is used for product list for 10 minutes.

## Development Notes

- Database schema is auto-created by Marten on startup.
- Health checks are exposed at `/health` and verify PostgreSQL and Redis connectivity.
- Logs are printed to console using Serilog.

## Troubleshooting

- If API fails to start due to DB connectivity, ensure Postgres and Redis are healthy (Docker health checks must pass).
- If product requests fail, verify the external API is reachable and adjust `ProductApi__BaseUrl` if needed.

## License

This project is for recruitment task purposes.
