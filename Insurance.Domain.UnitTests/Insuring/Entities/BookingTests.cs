using Insurance.Domain.Exceptions;
using Insurance.Domain.Insuring.Enums;

namespace Insurance.Domain.Insuring.Entities;

public class BookingTests
{
    [Fact]
    public void Constructor_ValidData_CreatesConfirmedBooking()
    {
        var booking = new Booking("REF123", new DateOnly(2024, 5, 1), new DateOnly(2024, 5, 5), 2);

        Assert.Equal("REF123", booking.Reference);
        Assert.Equal(new DateOnly(2024, 5, 1), booking.CheckIn);
        Assert.Equal(new DateOnly(2024, 5, 5), booking.CheckOut);
        Assert.Equal(2, booking.People);
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_InvalidReference_ThrowsDomainException(string reference)
    {
        Assert.Throws<DomainException>(() =>
            new Booking(reference, new DateOnly(2024, 5, 1), new DateOnly(2024, 5, 5), 2));
    }

    [Fact]
    public void Constructor_InvalidDates_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Booking("REF123", new DateOnly(2024, 5, 5), new DateOnly(2024, 5, 1), 2));
    }

    [Fact]
    public void Constructor_InvalidPeople_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Booking("REF123", new DateOnly(2024, 5, 1), new DateOnly(2024, 5, 5), 0));
    }

    [Fact]
    public void Modify_ValidData_UpdatesBookingAndStatus()
    {
        var booking = new Booking("REF123", new DateOnly(2024, 5, 1), new DateOnly(2024, 5, 5), 2);

        booking.Modify(new DateOnly(2024, 5, 2), new DateOnly(2024, 5, 6), 3);

        Assert.Equal(new DateOnly(2024, 5, 2), booking.CheckIn);
        Assert.Equal(new DateOnly(2024, 5, 6), booking.CheckOut);
        Assert.Equal(3, booking.People);
        Assert.Equal(BookingStatus.Modified, booking.Status);
    }

    [Fact]
    public void Cancel_ChangesStatusToCancelled()
    {
        var booking = new Booking("REF123", new DateOnly(2024, 5, 1), new DateOnly(2024, 5, 5), 2);

        booking.Cancel();

        Assert.Equal(BookingStatus.Cancelled, booking.Status);
    }

    [Fact]
    public void Cancel_AlreadyCancelled_ThrowsDomainException()
    {
        var booking = new Booking("REF123", new DateOnly(2024, 5, 1), new DateOnly(2024, 5, 5), 2);
        booking.Cancel();

        Assert.Throws<DomainException>(() => booking.Cancel());
    }

    [Fact]
    public void GetNights_ReturnsCorrectNumberOfNights()
    {
        var booking = new Booking("REF123", new DateOnly(2024, 5, 1), new DateOnly(2024, 5, 5), 2);

        Assert.Equal(4, booking.GetNights());
    }
}
