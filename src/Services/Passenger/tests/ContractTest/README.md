# Passenger contract tests

This project uses PactNet to define a consumer-driven contract for the Passenger API and then verify the running provider against the generated pact.

How it works:
- `PassengerContract` writes a pact file describing the expected `GET /api/v1/passenger/{id}` interaction.
- `PassengerProviderContractFixture` starts the real Passenger API on a TCP port and exposes `/provider-states` so Pact can prepare provider data before verification.
- `PassengerProviderContractTests` first generates the pact file, then runs Pact verifier against the live provider.

Provider state flow:
- The consumer interaction declares `There is a passenger with id ...`.
- During verification Pact calls `/provider-states`.
- `PassengerProviderStateSeeder` clears existing read-model data and inserts the passenger record required by that state.

Run locally:
```powershell
dotnet test src/Services/Passenger/tests/ContractTest/Contract.Test.csproj
```

Notes:
- PactNet provider verification requires a real socket host.
- Docker must be running because Passenger verification uses Postgres and MongoDB test containers.