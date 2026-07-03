using PactNet.Verifier;
using Xunit;

namespace Contract.Test;

public class BookingProviderContractTests : IClassFixture<BookingProviderContractFixture>
{
    private readonly BookingProviderContractFixture _fixture;

    public BookingProviderContractTests(BookingProviderContractFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task booking_api_honours_root_contract()
    {
        await BookingContract.WritePactAsync();

        var verifier = new PactVerifier(BookingContract.ProviderName, new PactVerifierConfig());

        verifier
            .WithHttpEndpoint(_fixture.ServerUri)
            .WithFileSource(new FileInfo(BookingContract.PactFilePath))
            .Verify();
    }
}