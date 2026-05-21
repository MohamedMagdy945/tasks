# .NET API — Logging with Seq (Zero to Hero)

A complete, production-ready reference implementation showing every layer of
professional structured logging in an ASP.NET Core API, with Seq as the log
server.

---

## What is Seq?

**Seq** is a self-hosted log server built specifically for structured logs.
Think of it as "Kibana, but simpler to set up and optimised for development
teams".  You write logs from your .NET app as structured JSON events; Seq
stores them and gives you a powerful search/filter/alert UI at
`http://localhost:8080`.

```
.NET API
  ↓  Serilog pipeline
  ↓  (enrichers → filters → sinks)
Seq Server          ← search, filter, alert
  http://localhost:8080
```

---

## Project Structure

```
SeqLoggingApi/
├── Program.cs                                  ← bootstrap + pipeline setup
├── appsettings.json                            ← Serilog + Seq config
├── appsettings.Production.json                 ← production overrides
├── docker-compose.yml                          ← one-command Seq server
│
├── Infrastructure/
│   ├── Logging/
│   │   ├── LoggingConfiguration.cs             ← all Serilog wiring
│   │   ├── LogContextExtensions.cs             ← push properties into context
│   │   └── LogMessages.cs                      ← [LoggerMessage] source gen
│   └── Middleware/
│       ├── GlobalExceptionMiddleware.cs         ← catch + log unhandled errors
│       └── CorrelationIdMiddleware.cs           ← X-Correlation-Id header
│
├── Features/
│   └── Orders/
│       ├── OrderService.cs                     ← business logic with logging
│       └── OrdersController.cs                 ← REST endpoints
│
└── Models/
    └── Order.cs                                ← domain + DTOs
```

---

## Quick Start

### 1 — Start Seq

```bash
docker-compose up -d
```

| URL | Purpose |
|-----|---------|
| `http://localhost:8080` | Seq web UI (login: admin / Admin1234!) |
| `http://localhost:5341` | Seq ingestion API (your app posts here) |

### 2 — Run the API

```bash
dotnet run
```

Swagger UI → `http://localhost:5017/swagger`

### 3 — Fire some requests

```bash
# Create an order
curl -X POST http://localhost:5017/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "C-001",
    "items": [
      { "productId": 1, "name": "Widget", "quantity": 2, "unitPrice": 9.99 }
    ]
  }'

# Retrieve it
curl http://localhost:5017/api/orders/1

# See all log levels in Seq
curl http://localhost:5017/api/orders/simulate-levels

# See how an unhandled exception is logged
curl http://localhost:5017/api/orders/simulate-error
```

Open `http://localhost:8080` — every request above will be a structured event
you can click into, filter on, and create alerts for.

---

## NuGet Packages

| Package | Why |
|---------|-----|
| `Serilog.AspNetCore` | Replaces Microsoft's default logger with Serilog |
| `Serilog.Sinks.Seq` | Ships log events to a Seq server |
| `Serilog.Enrichers.Environment` | Adds `MachineName`, `EnvironmentName` |
| `Serilog.Enrichers.Thread` | Adds `ThreadId` |
| `Serilog.Enrichers.Process` | Adds `ProcessId` |
| `Serilog.Enrichers.Span` | Adds `TraceId` + `SpanId` for distributed tracing |
| `Serilog.Settings.Configuration` | Reads Serilog config from `appsettings.json` |

---

## Key Concepts Implemented

### 1. Bootstrap Logger
A minimal console logger active from line 1 of `Program.cs` so that startup
crashes are never silently swallowed.

### 2. Full Serilog Pipeline (`LoggingConfiguration.cs`)
- Minimum level overrides per namespace
- Five enrichers (machine, environment, thread, process, span)
- Conditional sinks: human-readable console in dev, compact JSON in prod
- Seq sink with durable disk buffer + batching

### 3. Durable Buffer
Events are written to `./logs/seq-buffer` before being sent to Seq.
If Seq is temporarily unavailable the events survive and are sent when Seq
comes back — no data loss.

### 4. High-Performance `[LoggerMessage]` Source Generator
`LogMessages.cs` uses compile-time code generation (available since .NET 6).
The generated methods are ~10× faster than `_logger.LogInformation(...)` in
hot paths because they skip string formatting when the log level is disabled.

### 5. Correlation ID
Every request gets an `X-Correlation-Id` header (caller-supplied or generated).
It is pushed into `LogContext` so it appears on every log event for that
request. Consumers can pass their own ID; your logs and theirs align
automatically.

### 6. Global Exception Middleware
Catches anything that escapes controller try/catch, logs it with full context
(method, path, exception), and returns a clean RFC 7807 Problem Details JSON
instead of leaking stack traces.

### 7. Serilog Request Logging
One structured log event per HTTP request with method, path, status code,
and elapsed time — plus custom enrichment (host, user agent, user ID).

---

## Searching in Seq

Once events are flowing, try these queries in the Seq UI:

```sql
-- All errors
@Level = 'Error'

-- Errors from the order service
@Level = 'Error' AND SourceContext like '%Order%'

-- Follow a single request by correlation ID
CorrelationId = 'abc123'

-- Slow requests (if you log DurationMs)
DurationMs > 500

-- All events for a specific customer
CustomerId = 'C-001'

-- Validation failures
EventId.Id = 9001
```

---

## Production Checklist

- [ ] Set `SEQ__SERVERURL` and `SEQ__APIKEY` as environment variables
- [ ] Use `appsettings.Production.json` (minimum level `Warning`)
- [ ] Durable buffer path is on a persistent volume
- [ ] `Log.CloseAndFlushAsync()` called in the finally block ✓
- [ ] Sensitive fields never logged (no passwords, PII)
- [ ] Seq API key configured for authentication
- [ ] Alert rules set up for `@Level = 'Error'` spike
