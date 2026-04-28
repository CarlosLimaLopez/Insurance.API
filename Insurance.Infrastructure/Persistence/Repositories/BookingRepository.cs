using Insurance.Domain.Insuring.Entities;
using Insurance.Domain.Insuring.Interfaces;

namespace Insurance.Infrastructure.Persistence.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly InsuranceDbContext _dbContext;

    public BookingRepository(InsuranceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Booking booking)
    => await _dbContext.Bookings.AddAsync(booking);
    
    public async Task<Booking?> GetByReferenceAsync(string reference)
        => await _dbContext.Bookings.FindAsync(reference);
    
}