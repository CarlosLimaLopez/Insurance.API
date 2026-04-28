using Insurance.Application.Insuring.Commands;
using Insurance.Application.Insuring.Messages;
using Insurance.Domain.Insuring.Entities;
using Insurance.Domain.Insuring.Interfaces;
using Insurance.Domain.Interfaces;
using MassTransit;
using Moq;

namespace Insurance.Application.Insuring.Commands;

public class ConfirmBookingCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_PersistsEntitiesAndPublishesMessage()
    {
        // Arrange
        var bookingRepository = new Mock<IBookingRepository>();
        var insuredBookingRepository = new Mock<IInsuredBookingRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var publishEndpoint = new Mock<IPublishEndpoint>();

        var handler = new ConfirmBookingCommandHandler(
            bookingRepository.Object,
            insuredBookingRepository.Object,
            unitOfWork.Object,
            publishEndpoint.Object);

        var command = new ConfirmBookingCommand(
            "REF123",
            new DateOnly(2024, 5, 1),
            new DateOnly(2024, 5, 5),
            2);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        bookingRepository.Verify(r => r.AddAsync(It.Is<Booking>(b =>
            b.Reference == "REF123" &&
            b.CheckIn == new DateOnly(2024, 5, 1) &&
            b.CheckOut == new DateOnly(2024, 5, 5) &&
            b.People == 2
        )), Times.Once);

        insuredBookingRepository.Verify(r => r.AddAsync(It.Is<InsuredBooking>(ib =>
            ib.BookingReference == "REF123" &&
            ib.PremiumAmount == ((2 * 0.12m) + (4 * 0.08m))
        )), Times.Once);

        publishEndpoint.Verify(p => p.Publish(
            It.Is<IssueInsurancePolicyMessage>(msg =>
                msg.BookingReference == "REF123" &&
                msg.CheckIn == "2024-05-01" &&
                msg.CheckOut == "2024-05-05" &&
                msg.People == 2 &&
                msg.PremiumAmount == ((2 * 0.12m) + (4 * 0.08m)) &&
                msg.Currency == "EUR" &&
                msg.Product == "FLEXIBLE_INSURED"
            ),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}