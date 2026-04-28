using MediatR;

namespace Insurance.Application.Reporting.Queries;

public record CancelledBookingDto(
    string Reference,
    decimal PremiumAmount,
    string Currency,
    string? PolicyNumber);

public record GetCancelledBookingsQuery() : IRequest<IReadOnlyList<CancelledBookingDto>>;