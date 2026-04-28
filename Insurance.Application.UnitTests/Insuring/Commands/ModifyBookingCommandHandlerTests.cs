using Insurance.Application.Insuring.Commands;
using Insurance.Application.Insuring.Messages;
using Insurance.Domain.Exceptions;
using Insurance.Domain.Insuring.Entities;
using Insurance.Domain.Insuring.Enums;
using Insurance.Domain.Insuring.Interfaces;
using Insurance.Domain.Interfaces;
using MassTransit;
using Moq;

namespace Insurance.Application.Tests.Insuring.Commands;

public class ModifyBookingCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_UpdatesBookingAndPublishesMessage()
    {
        // Arrange
        var booking = new Booking("REF123", new DateOnly(2024, 5, 1), new DateOnly(2024, 5, 5), 2);
        var insuredBooking = new InsuredBooking("REF123", 2, 4);

        var bookingRepository = new Mock<IBookingRepository>();
        bookingRepository.Setup(r => r.GetByReferenceAsync("REF123")).ReturnsAsync(booking);

        var insuredBookingRepository = new Mock<IInsuredBookingRepository>();
        insuredBookingRepository.Setup(r => r.GetByBookingReferenceAsync("REF123")).ReturnsAsync(insuredBooking);

        var unitOfWork = new Mock<IUnitOfWork>();
        var publishEndpoint = new Mock<IPublishEndpoint>();

        var handler = new ModifyBookingCommandHandler(
            bookingRepository.Object,
            insuredBookingRepository.Object,
            unitOfWork.Object,
            publishEndpoint.Object);

        var command = new ModifyBookingCommand(
            "REF123",
            new DateOnly(2024, 5, 2),
            new DateOnly(2024, 5, 6),
            3);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(new DateOnly(2024, 5, 2), booking.CheckIn);
        Assert.Equal(new DateOnly(2024, 5, 6), booking.CheckOut);
        Assert.Equal(3, booking.People);
        Assert.Equal(BookingStatus.Modified, booking.Status);
        Assert.Equal((3 * 0.12m) + (4 * 0.08m), insuredBooking.PremiumAmount);

        publishEndpoint.Verify(p => p.Publish(
            It.Is<IssueInsurancePolicyMessage>(msg =>
                msg.BookingReference == "REF123" &&
                msg.CheckIn == "2024-05-02" &&
                msg.CheckOut == "2024-05-06" &&
                msg.People == 3 &&
                msg.PremiumAmount == ((3 * 0.12m) + (4 * 0.08m)) &&
                msg.Currency == "EUR" &&
                msg.Product == "FLEXIBLE_INSURED"
            ),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ThrowsDomainException()
    {
        var bookingRepository = new Mock<IBookingRepository>();
        bookingRepository.Setup(r => r.GetByReferenceAsync("NOT_FOUND")).ReturnsAsync((Booking?)null);

        var insuredBookingRepository = new Mock<IInsuredBookingRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var publishEndpoint = new Mock<IPublishEndpoint>();

        var handler = new ModifyBookingCommandHandler(
            bookingRepository.Object,
            insuredBookingRepository.Object,
            unitOfWork.Object,
            publishEndpoint.Object);

        var command = new ModifyBookingCommand(
            "NOT_FOUND",
            new DateOnly(2024, 5, 2),
            new DateOnly(2024, 5, 6),
            3);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InsuredBookingNotFound_ThrowsDomainException()
    {
        var booking = new Booking("REF123", new DateOnly(2024, 5, 1), new DateOnly(2024, 5, 5), 2);
        var bookingRepository = new Mock<IBookingRepository>();
        bookingRepository.Setup(r => r.GetByReferenceAsync("REF123")).ReturnsAsync(booking);

        var insuredBookingRepository = new Mock<IInsuredBookingRepository>();
        insuredBookingRepository.Setup(r => r.GetByBookingReferenceAsync("REF123")).ReturnsAsync((InsuredBooking?)null);

        var unitOfWork = new Mock<IUnitOfWork>();
        var publishEndpoint = new Mock<IPublishEndpoint>();

        var handler = new ModifyBookingCommandHandler(
            bookingRepository.Object,
            insuredBookingRepository.Object,
            unitOfWork.Object,
            publishEndpoint.Object);

        var command = new ModifyBookingCommand(
            "REF123",
            new DateOnly(2024, 5, 2),
            new DateOnly(2024, 5, 6),
            3);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}