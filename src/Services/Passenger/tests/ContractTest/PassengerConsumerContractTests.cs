using Xunit;

namespace Contract.Test;

public class PassengerConsumerContractTests
{
    [Fact]
    public async Task get_passenger_by_id_contract_is_generated()
    {
        await PassengerContract.WritePactAsync();

        Assert.True(File.Exists(PassengerContract.PactFilePath));
    }
}