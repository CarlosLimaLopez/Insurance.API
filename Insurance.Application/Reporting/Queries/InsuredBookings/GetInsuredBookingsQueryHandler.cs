using Insurance.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Application.Reporting.Queries;

public class GetInsuredBookingsQueryHandler : IRequestHandler<GetInsuredBookingsQuery, IReadOnlyList<InsuredBookingDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetInsuredBookingsQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<IReadOnlyList<InsuredBookingDto>> Handle(GetInsuredBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _readDbContext.InsuredBookings
            .AsNoTracking()
            .Where(ib => !ib.IsCancelled)
            .Select(ib => new InsuredBookingDto(
                ib.BookingReference,
                ib.PremiumAmount,
                ib.Currency,
                ib.PolicyNumber))
            .ToListAsync(cancellationToken);

        return bookings;
    }
}