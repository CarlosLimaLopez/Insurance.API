using MediatR;

namespace Insurance.Application.Insuring.Commands;

public record ModifyBookingCommand(
    string Reference,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int People) : IRequest;