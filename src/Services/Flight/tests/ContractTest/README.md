# Flight contract tests

This project uses PactNet to define and verify an HTTP contract for the Flight service.

How it works:
- `FlightContract` writes a pact file from a consumer-style test.
- `FlightProviderContractFixture` boots the real Flight API on a TCP port and provisions required infrastructure with Testcontainers.
- `FlightProviderContractTests` verifies the running provider against the generated pact file.

Current coverage:
- The contract exercises `GET /`, which is a stable service-level endpoint and keeps provider verification fast and deterministic.

Run locally:
```powershell
dotnet test src/Services/Flight/tests/ContractTest/Contract.Test.csproj
```

Notes:
- Pact provider verification cannot run against `TestServer` or `WebApplicationFactory`; it must hit a real HTTP socket.
- Docker must be available because Flight startup depends on Postgres, MongoDB, RabbitMQ, and EventStoreDB.