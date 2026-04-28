using MediatR;

namespace Insurance.Application.Insuring.Commands;

public record CancelBookingCommand(string Reference) : IRequest;