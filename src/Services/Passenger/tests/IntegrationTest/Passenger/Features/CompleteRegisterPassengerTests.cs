using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using FluentAssertions;
using Integration.Test.Fakes;
using Microsoft.Extensions.Logging;
using Passenger.Data;
using Xunit;
using Xunit.Abstractions;

namespace Integration.Test.Passenger.Features;

public class CompleteRegisterPassengerTests : PassengerIntegrationTestBase
{
    public CompleteRegisterPassengerTests(
        TestFixture<Program, PassengerDbContext, PassengerReadDbContext> integrationTestFactory,
        ITestOutputHelper outputHelper
    )
        : base(integrationTestFactory)
    {
        Fixture.Logger = Fixture.CreateLogger(outputHelper);
    }

    [Fact]
    public async Task should_complete_register_passenger_and_update_to_db()
    {
        // Arrange
        Fixture.Logger?.LogInformation("Starting CompleteRegisterPassenger test at {Time}", DateTime.UtcNow);

        try
        {
            // Generate and publish UserCreated event
            var userCreated = new FakeUserCreated().Generate();
            Fixture.Logger?.LogInformation(
                "Generated UserCreated event with PassportNumber: {PassportNumber}",
                userCreated.PassportNumber
            );

            await Fixture.Publish(userCreated);
            Fixture.Logger?.LogInformation("Published UserCreated event");

            // Wait for publishing with retry logic
            var published = await WaitForWithRetry(
                async () => await Fixture.WaitForPublishing<UserCreated>(),
                "publishing",
                maxRetries: 3
            );

            published.Should().BeTrue("UserCreated event should be published to message broker");
            Fixture.Logger?.LogInformation("UserCreated event published successfully");

            // Wait for consuming with retry logic
            var consumed = await WaitForWithRetry(
                async () => await Fixture.WaitForConsuming<UserCreated>(),
                "consuming",
                maxRetries: 5
            );

            consumed.Should().BeTrue("UserCreated event should be consumed by the passenger service");
            Fixture.Logger?.LogInformation("UserCreated event consumed successfully");

            // Small delay to ensure event processing is complete
            await Task.Delay(1000);

            // Generate and send complete registration command
            var command = new FakeCompleteRegisterPassengerCommand(userCreated.PassportNumber).Generate();
            Fixture.Logger?.LogInformation(
                "Sending CompleteRegisterPassenger command for PassportNumber: {PassportNumber}",
                command.PassportNumber
            );

            // Act
            var response = await Fixture.SendAsync(command);
            Fixture.Logger?.LogInformation("Received response for CompleteRegisterPassenger command");

            // Assert with detailed logging
            response.Should().NotBeNull("Response should not be null");
            Fixture.Logger?.LogInformation("Response is not null");

            response?.PassengerDto.Should().NotBeNull("PassengerDto should not be null");

            response
                ?.PassengerDto?.Name.Should()
                .Be(
                    userCreated.Name,
                    $"Passenger name should be '{userCreated.Name}' but was '{response?.PassengerDto?.Name}'"
                );

            response
                ?.PassengerDto?.PassportNumber.Should()
                .Be(
                    command.PassportNumber,
                    $"Passport number should be '{command.PassportNumber}' but was '{response?.PassengerDto?.PassportNumber}'"
                );

            response
                ?.PassengerDto?.PassengerType.ToString()
                .Should()
                .Be(
                    command.PassengerType.ToString(),
                    $"Passenger type should be '{command.PassengerType}' but was '{response?.PassengerDto?.PassengerType}'"
                );

            response
                ?.PassengerDto?.Age.Should()
                .Be(command.Age, $"Age should be {command.Age} but was {response?.PassengerDto?.Age}");

            Fixture.Logger?.LogInformation("All assertions passed successfully");
        }
        catch (Exception ex)
        {
            Fixture.Logger?.LogError(ex, "Test failed with exception: {Message}", ex.Message);
            throw;
        }
        finally
        {
            Fixture.Logger?.LogInformation("Test completed at {Time}", DateTime.UtcNow);
        }
    }

    private async Task<bool> WaitForWithRetry(Func<Task<bool>> waitCondition, string operation, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            Fixture.Logger?.LogInformation(
                "Attempt {Attempt}/{MaxRetries} for {Operation}",
                i + 1,
                maxRetries,
                operation
            );

            var result = await waitCondition();

            if (result)
            {
                Fixture.Logger?.LogInformation("{Operation} successful on attempt {Attempt}", operation, i + 1);
                return true;
            }

            if (i < maxRetries - 1)
            {
                var delaySeconds = (i + 1) * 2; // Exponential backoff: 2, 4, 6 seconds
                Fixture.Logger?.LogWarning(
                    "{Operation} failed on attempt {Attempt}, waiting {Delay}s before retry",
                    operation,
                    i + 1,
                    delaySeconds
                );
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }
        }

        Fixture.Logger?.LogError("{Operation} failed after {MaxRetries} attempts", operation, maxRetries);
        return false;
    }
}
