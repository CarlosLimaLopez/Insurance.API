using Insurance.Application.Insuring.Commands;
using Insurance.Domain.Exceptions;
using Insurance.Domain.Insuring.Interfaces;
using Insurance.Domain.Interfaces;
using MediatR;

namespace Insurance.Application.Insuring;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IInsuredBookingRepository _insuredBookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelBookingCommandHandler(
        IBookingRepository bookingRepository,
        IInsuredBookingRepository insuredBookingRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _insuredBookingRepository = insuredBookingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByReferenceAsync(request.Reference);
        var insuredBooking = await _insuredBookingRepository.GetByBookingReferenceAsync(request.Reference);

        if (booking == null || insuredBooking == null)
            throw new DomainException($"Insured booking not found with the reference: {request.Reference}");
        
        booking.Cancel();
        insuredBooking.MarkAsCancelled();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}