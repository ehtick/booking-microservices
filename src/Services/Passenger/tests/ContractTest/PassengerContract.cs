using System.Net;
using System.Net.Http.Json;
using PactNet;
using Xunit;

namespace Contract.Test;

internal static class PassengerContract
{
    public const string ConsumerName = "Booking Client";
    public const string ProviderName = "Passenger API";
    public const string AuthorizationHeaderValue = "Bearer contract-test-token";
    public const string ProviderState = "There is a passenger with id 11111111-1111-1111-1111-111111111111";
    public static readonly Guid PassengerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public const string PassengerName = "Sam Smith";
    public const string PassportNumber = "P123456789";
    public const string PassengerType = "Male";
    public const int Age = 29;

    public static string PactDirectory => Path.Combine(ProjectDirectory, "pacts");
    public static string LogDirectory => Path.Combine(ProjectDirectory, "logs");
    public static string PactFilePath => Path.Combine(PactDirectory, $"{ConsumerName}-{ProviderName}.json");
    public static string ProjectDirectory => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    public static async Task WritePactAsync()
    {
        var pact = Pact.V4(
            ConsumerName,
            ProviderName,
            new PactConfig
            {
                PactDir = PactDirectory,
            });

        var pactBuilder = pact.WithHttpInteractions();

        pactBuilder
            .UponReceiving("A request to retrieve a passenger by id")
            .Given(ProviderState)
            .WithRequest(HttpMethod.Get, $"/api/v1/passenger/{PassengerId}")
            .WithHeader("Authorization", AuthorizationHeaderValue)
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new
            {
                passengerDto = new
                {
                    id = PassengerId,
                    name = PassengerName,
                    passportNumber = PassportNumber,
                    passengerType = PassengerType,
                    age = Age,
                },
            });

        await pactBuilder.VerifyAsync(async context =>
        {
            using var httpClient = new HttpClient { BaseAddress = context.MockServerUri };
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", AuthorizationHeaderValue);

            var response = await httpClient.GetFromJsonAsync<GetPassengerByIdResponseDto>($"/api/v1/passenger/{PassengerId}");

            Assert.NotNull(response);
            Assert.Equal(PassengerId, response!.PassengerDto.Id);
            Assert.Equal(PassengerName, response.PassengerDto.Name);
            Assert.Equal(PassportNumber, response.PassengerDto.PassportNumber);
            Assert.Equal(PassengerType, response.PassengerDto.PassengerType);
            Assert.Equal(Age, response.PassengerDto.Age);
        });
    }
}

internal sealed record GetPassengerByIdResponseDto(ContractPassengerDto PassengerDto);

internal sealed record ContractPassengerDto(Guid Id, string Name, string PassportNumber, string PassengerType, int Age);