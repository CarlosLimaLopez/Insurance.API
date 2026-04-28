using Insurance.Domain.Exceptions;

namespace Insurance.Domain.Insuring.Entities;

/// <summary>
/// Represents an insured hotel booking, including premium calculation and policy assignment.
/// </summary>
public class InsuredBooking
{
    /// <summary>
    /// Cost per person for the insurance premium calculation.
    /// </summary>
    private const decimal CostPerPerson = 0.12m;

    /// <summary>
    /// Cost per night for the insurance premium calculation.
    /// </summary>
    private const decimal CostPerNight = 0.08m;

    /// <summary>
    /// Unique identifier for the insured booking.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Reference to the associated hotel booking.
    /// </summary>
    public string BookingReference { get; private set; }

    /// <summary>
    /// Calculated insurance premium amount.
    /// </summary>
    public decimal PremiumAmount { get; private set; }

    /// <summary>
    /// Currency for the premium amount (default: EUR).
    /// </summary>
    public string Currency { get; private set; } = "EUR";

    /// <summary>
    /// Insurance product name (default: FLEXIBLE_INSURED).
    /// </summary>
    public string Product { get; private set; } = "FLEXIBLE_INSURED";

    /// <summary>
    /// Policy number assigned by the insurance provider.
    /// </summary>
    public string? PolicyNumber { get; private set; }

    /// <summary>
    /// Indicates if the insured booking has been cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private InsuredBooking() { }

    /// <summary>
    /// Creates a new insured booking and calculates the premium.
    /// </summary>
    /// <param name="bookingReference">Reference to the hotel booking.</param>
    /// <param name="people">Number of people in the booking.</param>
    /// <param name="nights">Number of nights for the booking.</param>
    public InsuredBooking(string bookingReference, int people, int nights)
    {
        ValidateInputs(bookingReference, people, nights);

        Id = Guid.NewGuid();
        BookingReference = bookingReference;
        PremiumAmount = CalculatePremium(people, nights);
    }

    /// <summary>
    /// Assigns a policy number to the insured booking.
    /// </summary>
    /// <param name="policyNumber">Policy number from the insurance provider.</param>
    public void AssignPolicy(string policyNumber)
    {
        if (string.IsNullOrWhiteSpace(policyNumber))
            throw new DomainException("Policy number cannot be null or empty.");

        PolicyNumber = policyNumber;
    }

    /// <summary>
    /// Recalculates the premium amount based on updated booking details.
    /// </summary>
    /// <param name="newPeople">Updated number of people.</param>
    /// <param name="newNights">Updated number of nights.</param>
    public void RecalculatePremium(int newPeople, int newNights)
    {
        ValidateInputs(BookingReference, newPeople, newNights);

        PremiumAmount = CalculatePremium(newPeople, newNights);
    }

    /// <summary>
    /// Marks the insured booking as cancelled.
    /// </summary>
    public void MarkAsCancelled()
    {
        if (IsCancelled)
            throw new DomainException("The insured booking is already marked as cancelled.");

        IsCancelled = true;
    }

    /// <summary>
    /// Calculates the insurance premium based on people and nights.
    /// </summary>
    private static decimal CalculatePremium(int people, int nights)
    {
        return (people * CostPerPerson) + (nights * CostPerNight);
    }

    /// <summary>
    /// Validates the input values for the insured booking.
    /// </summary>
    private static void ValidateInputs(string bookingReference, int people, int nights)
    {
        if (string.IsNullOrWhiteSpace(bookingReference))
            throw new DomainException("Booking reference cannot be null or empty.");

        if (people <= 0)
            throw new DomainException("A booking must include at least one person.");

        if (nights <= 0)
            throw new DomainException("A booking must be for at least one night.");
    }
}