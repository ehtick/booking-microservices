using PactNet.Verifier;
using Xunit;

namespace Contract.Test;

public class IdentityProviderContractTests : IClassFixture<IdentityProviderContractFixture>
{
    private readonly IdentityProviderContractFixture _fixture;

    public IdentityProviderContractTests(IdentityProviderContractFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task identity_api_honours_root_contract()
    {
        await IdentityContract.WritePactAsync();

        var verifier = new PactVerifier(IdentityContract.ProviderName, new PactVerifierConfig());

        verifier
            .WithHttpEndpoint(_fixture.ServerUri)
            .WithFileSource(new FileInfo(IdentityContract.PactFilePath))
            .Verify();
    }
}