using MediatR;

namespace Insurance.Application.Reporting.Queries;

public record InsuredBookingDto(
    string Reference,
    decimal PremiumAmount,
    string Currency,
    string? PolicyNumber);

public record GetInsuredBookingsQuery() : IRequest<IReadOnlyList<InsuredBookingDto>>;