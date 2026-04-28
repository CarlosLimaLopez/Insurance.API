using Insurance.Application.Insuring.Messages;
using Insurance.Domain.Exceptions;
using Insurance.Domain.Insuring.Interfaces;
using Insurance.Domain.Interfaces;
using MassTransit;
using MediatR;

namespace Insurance.Application.Insuring.Commands;

public class ModifyBookingCommandHandler : IRequestHandler<ModifyBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IInsuredBookingRepository _insuredBookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public ModifyBookingCommandHandler(
        IBookingRepository bookingRepository,
        IInsuredBookingRepository insuredBookingRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _bookingRepository = bookingRepository;
        _insuredBookingRepository = insuredBookingRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(ModifyBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByReferenceAsync(request.Reference);
        var insuredBooking = await _insuredBookingRepository.GetByBookingReferenceAsync(request.Reference);

        if (booking == null)
            throw new DomainException($"Booking not found with the reference: {request.Reference}");

        if (insuredBooking == null)
            throw new DomainException($"Insured booking not found with the reference: {request.Reference}");

        booking.Modify(request.CheckIn, request.CheckOut, request.People);
        insuredBooking.RecalculatePremium(booking.People, booking.GetNights());

        var message = new IssueInsurancePolicyMessage(
            booking.Reference,
            booking.CheckIn.ToString("yyyy-MM-dd"),
            booking.CheckOut.ToString("yyyy-MM-dd"),
            booking.People,
            insuredBooking.PremiumAmount,
            insuredBooking.Currency,
            insuredBooking.Product
        );

        await _publishEndpoint.Publish(message, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}