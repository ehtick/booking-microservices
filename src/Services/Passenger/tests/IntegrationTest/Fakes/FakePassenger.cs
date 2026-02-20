using AutoBogus;
using Passenger.Passengers.ValueObjects;

namespace Integration.Test.Fakes;

using MassTransit;

public class FakePassenger : AutoFaker<global::Passenger.Passengers.Models.Passenger>
{
    public FakePassenger()
    {
        RuleFor(r => r.Id, _ => PassengerId.Of(NewId.NextGuid()));
        RuleFor(r => r.Name, _ => Name.Of("Sam"));
        RuleFor(r => r.PassportNumber, _ => PassportNumber.Of("123456789"));
    }
}
