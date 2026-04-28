using Insurance.Application.Insuring.Messages;
using Insurance.Domain.Insuring.Entities;
using Insurance.Domain.Insuring.Interfaces;
using Insurance.Domain.Interfaces;
using MassTransit;
using MediatR;

namespace Insurance.Application.Insuring.Commands;

public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IInsuredBookingRepository _insuredBookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public ConfirmBookingCommandHandler(
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

    public async Task Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = new Booking(request.Reference, request.CheckIn, request.CheckOut, request.People);
        var insuredBooking = new InsuredBooking(booking.Reference, booking.People, booking.GetNights());

        await _bookingRepository.AddAsync(booking);
        await _insuredBookingRepository.AddAsync(insuredBooking);

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