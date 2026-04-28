using Insurance.Domain.Insuring.Entities;

namespace Insurance.Application.Interfaces;

public interface IReadDbContext
{
    IQueryable<Booking> Bookings { get; }
    IQueryable<InsuredBooking> InsuredBookings { get; }
}