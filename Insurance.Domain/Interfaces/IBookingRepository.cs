using Insurance.Domain.Insuring.Entities;

namespace Insurance.Domain.Insuring.Interfaces;

public interface IBookingRepository
{
    Task AddAsync(Booking booking);
    Task<Booking?> GetByReferenceAsync(string reference);
}