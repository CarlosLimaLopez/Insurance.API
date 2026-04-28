using MediatR;

namespace Insurance.Application.Insuring.Commands;

public record ConfirmBookingCommand(
    string Reference,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int People) : IRequest;