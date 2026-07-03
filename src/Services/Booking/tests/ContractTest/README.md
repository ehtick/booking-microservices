# Booking contract tests

This project uses PactNet to create and verify an HTTP contract for the Booking service.

How it works:
- `BookingContract` acts as the consumer test. It defines expected request and response behaviour and writes a pact file into `pacts/`.
- `BookingProviderContractFixture` boots the real Booking API on a TCP port. Pact provider verification requires a real socket host, not `WebApplicationFactory` or `TestServer`.
- `BookingProviderContractTests` loads the generated pact file and verifies that the running provider matches it.

Why this contract is simple:
- Booking API startup depends on Postgres, MongoDB, RabbitMQ, and EventStoreDB, so the fixture provisions those dependencies with Testcontainers.
- The contract currently covers the root endpoint (`GET /`) as a stable service-level contract that can run without mocking downstream gRPC dependencies.

Run locally:
```powershell
dotnet test src/Services/Booking/tests/ContractTest/Contract.Test.csproj
```

Notes:
- PactNet 5 uses a native backend and needs a real TCP listener.
- Windows x64 is supported by PactNet. Ensure Docker is running before executing provider verification.