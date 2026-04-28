using Insurance.Domain.Exceptions;

namespace Insurance.Domain.Insuring.Entities;

public class InsuredBookingTests
{
    [Fact]
    public void Constructor_ValidData_CalculatesPremium()
    {
        var insured = new InsuredBooking("REF123", 2, 3);

        Assert.Equal("REF123", insured.BookingReference);
        Assert.Equal((2 * 0.12m) + (3 * 0.08m), insured.PremiumAmount);
        Assert.Equal("EUR", insured.Currency);
        Assert.Equal("FLEXIBLE_INSURED", insured.Product);
        Assert.Null(insured.PolicyNumber);
        Assert.False(insured.IsCancelled);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_InvalidReference_ThrowsDomainException(string reference)
    {
        Assert.Throws<DomainException>(() => new InsuredBooking(reference, 2, 3));
    }

    [Fact]
    public void Constructor_InvalidPeople_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new InsuredBooking("REF123", 0, 3));
    }

    [Fact]
    public void Constructor_InvalidNights_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new InsuredBooking("REF123", 2, 0));
    }

    [Fact]
    public void AssignPolicy_ValidNumber_SetsPolicyNumber()
    {
        var insured = new InsuredBooking("REF123", 2, 3);
        insured.AssignPolicy("POL123");

        Assert.Equal("POL123", insured.PolicyNumber);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AssignPolicy_InvalidNumber_ThrowsDomainException(string policy)
    {
        var insured = new InsuredBooking("REF123", 2, 3);

        Assert.Throws<DomainException>(() => insured.AssignPolicy(policy));
    }

    [Fact]
    public void RecalculatePremium_UpdatesPremium()
    {
        var insured = new InsuredBooking("REF123", 2, 3);
        insured.RecalculatePremium(4, 5);

        Assert.Equal((4 * 0.12m) + (5 * 0.08m), insured.PremiumAmount);
    }

    [Fact]
    public void MarkAsCancelled_SetsIsCancelled()
    {
        var insured = new InsuredBooking("REF123", 2, 3);
        insured.MarkAsCancelled();

        Assert.True(insured.IsCancelled);
    }

    [Fact]
    public void MarkAsCancelled_AlreadyCancelled_ThrowsDomainException()
    {
        var insured = new InsuredBooking("REF123", 2, 3);
        insured.MarkAsCancelled();

        Assert.Throws<DomainException>(() => insured.MarkAsCancelled());
    }
}
