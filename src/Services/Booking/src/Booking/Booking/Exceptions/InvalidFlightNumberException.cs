using BuildingBlocks.Exception;

namespace Booking.Booking.Exceptions;

public class InvalidFlightNumberException : DomainException
{
    public InvalidFlightNumberException(string flightNumber)
        : base($"Flight Number: '{flightNumber}' is invalid.") { }
}
