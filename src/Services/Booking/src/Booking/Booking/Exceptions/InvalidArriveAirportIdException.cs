using BuildingBlocks.Exception;

namespace Booking.Booking.Exceptions;

public class InvalidArriveAirportIdException : DomainException
{
    public InvalidArriveAirportIdException(Guid arriveAirportId)
        : base($"arriveAirportId: '{arriveAirportId}' is invalid.") { }
}
