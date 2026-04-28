using Insurance.Domain.Insuring.Entities;
using Insurance.Domain.Insuring.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Infrastructure.Persistence.Repositories;

public class InsuredBookingRepository : IInsuredBookingRepository
{
    private readonly InsuranceDbContext _dbContext;

    public InsuredBookingRepository(InsuranceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(InsuredBooking insuredBooking)
        => await _dbContext.InsuredBookings.AddAsync(insuredBooking);
    
    public async Task<InsuredBooking?> GetByBookingReferenceAsync(string bookingReference)
        => await _dbContext.InsuredBookings.FirstOrDefaultAsync(x => x.BookingReference == bookingReference);
}