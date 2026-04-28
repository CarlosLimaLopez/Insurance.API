using Insurance.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Application.Reporting.Queries;

public class GetCancelledBookingsQueryHandler : IRequestHandler<GetCancelledBookingsQuery, IReadOnlyList<CancelledBookingDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetCancelledBookingsQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<IReadOnlyList<CancelledBookingDto>> Handle(GetCancelledBookingsQuery request, CancellationToken cancellationToken)
    {
        var insuredBookings = await _readDbContext.InsuredBookings
            .AsNoTracking()
            .Where(ib => ib.IsCancelled)
            .Select(ib => new CancelledBookingDto(
                ib.BookingReference,
                ib.PremiumAmount,
                ib.Currency,
                ib.PolicyNumber))
            .ToListAsync(cancellationToken);

        return insuredBookings;
    }
}