using Insurance.Application.Insuring;
using Insurance.Application.Insuring.Commands;
using Insurance.Domain.Exceptions;
using Insurance.Domain.Insuring.Entities;
using Insurance.Domain.Insuring.Enums;
using Insurance.Domain.Insuring.Interfaces;
using Insurance.Domain.Interfaces;
using Moq;

namespace Insurance.Application.Tests.Insuring.Commands;

public class CancelBookingCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CancelsBookingAndInsuredBooking()
    {
        var booking = new Booking("REF123", new DateOnly(2024, 5, 1), new DateOnly(2024, 5, 5), 2);
        var insuredBooking = new InsuredBooking("REF123", 2, 4);

        var bookingRepository = new Mock<IBookingRepository>();
        bookingRepository.Setup(r => r.GetByReferenceAsync("REF123")).ReturnsAsync(booking);

        var insuredBookingRepository = new Mock<IInsuredBookingRepository>();
        insuredBookingRepository.Setup(r => r.GetByBookingReferenceAsync("REF123")).ReturnsAsync(insuredBooking);

        var unitOfWork = new Mock<IUnitOfWork>();

        var handler = new CancelBookingCommandHandler(
            bookingRepository.Object,
            insuredBookingRepository.Object,
            unitOfWork.Object);

        var command = new CancelBookingCommand("REF123");

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(BookingStatus.Cancelled, booking.Status);
        Assert.True(insuredBooking.IsCancelled);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_BookingOrInsuredBookingNotFound_ThrowsDomainException()
    {
        var bookingRepository = new Mock<IBookingRepository>();
        bookingRepository.Setup(r => r.GetByReferenceAsync("NOT_FOUND")).ReturnsAsync((Booking?)null);

        var insuredBookingRepository = new Mock<IInsuredBookingRepository>();
        insuredBookingRepository.Setup(r => r.GetByBookingReferenceAsync("NOT_FOUND")).ReturnsAsync((InsuredBooking?)null);

        var unitOfWork = new Mock<IUnitOfWork>();

        var handler = new CancelBookingCommandHandler(
            bookingRepository.Object,
            insuredBookingRepository.Object,
            unitOfWork.Object);

        var command = new CancelBookingCommand("NOT_FOUND");

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}