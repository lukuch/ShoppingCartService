# Architectural Decisions and Assumptions (ADR)

This document outlines key assumptions and decisions made to complete the recruitment task.

## External Product API Host
- Assumption: `https://fakeapi.platzi.com` is currently hosted under `https://api.escuelajs.co`.
- Rationale: Platzi Fake Store API currently resolves/redirects to `api.escuelajs.co`. The base URL is configurable via `ProductApi__BaseUrl` to accommodate changes.

## User Identity Model
- Assumption: `userId` is provided as a path parameter and is not authenticated.
- Rationale: The task does not require auth. Using a simple string `userId` unblocks cart operations without adding auth complexity.

## Persistence
- Decision: Use Marten (PostgreSQL) as document storage for the cart aggregate.
- Rationale: Marten provides simple document persistence with optional eventing, suits a lightweight cart use case, and is easy to run in Docker.

## Caching
- Decision: Cache product list in Redis for 10 minutes.
- Rationale: External catalog is read-heavy and relatively static; caching improves latency and reduces external API calls.

## Cart Business Rules
- Decision: Basic limits included in domain:
  - Max 50 distinct items per cart
  - Max 10 units per item per operation
  - Max cart value 10,000
  - Tax rate 23%
- Rationale: Provides realistic guardrails and a deterministic summary. Values are centralized in the domain for clarity and can be configurable later.

## Error Handling for External API
- Decision: Use Refit + Polly with exponential backoff for external calls; propagate status and body on failure.
- Rationale: Improves resilience and returns useful error information to clients.

## API Shape
- Decision: Minimal REST endpoints using Carter groups under `/api/products` and `/api/cart/{userId}`.
- Rationale: Meets task requirements with a clean, discoverable surface and Swagger documentation.

## Health and Observability
- Decision: Expose `/health` and use Serilog console logging.
- Rationale: Simple operational readiness checks and structured logs suitable for local and containerized runs.

## Dockerized Local Environment
- Decision: Provide `docker-compose.yml` for API, PostgreSQL, and Redis.
- Rationale: One-command local setup to simplify evaluation and testing.

## Production Considerations (Intentionally Omitted)

Kept minimal on purpose for the recruitment task. Most critical omissions for a real production system:

- Frontend UI: not included; this repo is backend-only.
- Authentication/Authorization: none; endpoints are open for demo purposes.
- Automated tests: no unit, integration, E2E tests.
- CI/CD & IaC: no pipelines or infrastructure-as-code; local Docker Compose only.
- Pagination: products listing is not paginated; add pagination and max page size.
- Observability: minimal logging only; no tracing/metrics, no rate limiting.
- Security/CORS: no CORS policy, security headers (HSTS/CSP), or secrets management.