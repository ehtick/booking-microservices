using BookingMonolith.Booking.Bookings.Models;
using BuildingBlocks.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookingMonolith.Booking.Data;


public class BookingReadDbContext : MongoDbContext
{
    public BookingReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Booking = GetCollection<BookingReadModel>(nameof(Booking).Underscore());
    }

    public IMongoCollection<BookingReadModel> Booking { get; }
}
