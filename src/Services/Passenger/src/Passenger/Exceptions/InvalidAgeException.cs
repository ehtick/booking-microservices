using BuildingBlocks.Exception;

namespace Passenger.Exceptions;

public class InvalidAgeException : DomainException
{
    public InvalidAgeException()
        : base("Age Cannot be null or negative") { }
}
