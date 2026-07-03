using PactNet.Verifier;
using Xunit;

namespace Contract.Test;

public class PassengerProviderContractTests : IClassFixture<PassengerProviderContractFixture>
{
    private readonly PassengerProviderContractFixture _fixture;

    public PassengerProviderContractTests(PassengerProviderContractFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task passenger_api_honours_passenger_contract()
    {
        await PassengerContract.WritePactAsync();

        var verifier = new PactVerifier(PassengerContract.ProviderName, new PactVerifierConfig());

        verifier
            .WithHttpEndpoint(_fixture.ServerUri)
            .WithFileSource(new FileInfo(PassengerContract.PactFilePath))
            .WithProviderStateUrl(new Uri(_fixture.ServerUri, "/provider-states"))
            .Verify();
    }
}