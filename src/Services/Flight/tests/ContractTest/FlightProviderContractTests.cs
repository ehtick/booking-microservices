using PactNet.Verifier;
using Xunit;

namespace Contract.Test;

public class FlightProviderContractTests : IClassFixture<FlightProviderContractFixture>
{
    private readonly FlightProviderContractFixture _fixture;

    public FlightProviderContractTests(FlightProviderContractFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task flight_api_honours_root_contract()
    {
        await FlightContract.WritePactAsync();

        var verifier = new PactVerifier(FlightContract.ProviderName, new PactVerifierConfig());

        verifier
            .WithHttpEndpoint(_fixture.ServerUri)
            .WithFileSource(new FileInfo(FlightContract.PactFilePath))
            .Verify();
    }
}