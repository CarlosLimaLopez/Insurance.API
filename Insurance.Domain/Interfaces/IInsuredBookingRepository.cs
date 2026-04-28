using Insurance.Domain.Insuring.Entities;

namespace Insurance.Domain.Insuring.Interfaces;

public interface IInsuredBookingRepository
{
    Task AddAsync(InsuredBooking insuredBooking);
    Task<InsuredBooking?> GetByBookingReferenceAsync(string bookingReference);
}