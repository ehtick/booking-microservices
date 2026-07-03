using System.Net;
using PactNet;
using Xunit;

namespace Contract.Test;

internal static class IdentityContract
{
    public const string ConsumerName = "Platform Smoke Client";
    public const string ProviderName = "Identity API";
    public const string AppName = "Identity.Api.Contract";

    public static string PactDirectory => Path.Combine(ProjectDirectory, "pacts");
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
            .UponReceiving("A request for the identity service health page")
            .WithRequest(HttpMethod.Get, "/")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithBody(AppName, "text/plain; charset=utf-8");

        await pactBuilder.VerifyAsync(async context =>
        {
            using var httpClient = new HttpClient { BaseAddress = context.MockServerUri };

            var response = await httpClient.GetStringAsync("/");

            Assert.Equal(AppName, response);
        });
    }
}