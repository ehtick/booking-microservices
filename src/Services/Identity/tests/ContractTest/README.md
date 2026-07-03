# Identity contract tests

This project uses PactNet to define and verify an HTTP contract for the Identity service.

How it works:
- `IdentityContract` generates a pact file from a consumer-side interaction definition.
- `IdentityProviderContractFixture` runs the actual Identity API on a TCP port with containerised infrastructure.
- `IdentityProviderContractTests` verifies the provider against the generated pact.

Current coverage:
- The contract targets `GET /` as a stable provider contract that validates the service is reachable and returning the configured service name.

Run locally:
```powershell
dotnet test src/Services/Identity/tests/ContractTest/Contract.Test.csproj
```

Notes:
- Use a real HTTP listener for Pact verification; in-memory ASP.NET Core test servers are not supported.
- Docker is required because Identity startup still needs Postgres and RabbitMQ.