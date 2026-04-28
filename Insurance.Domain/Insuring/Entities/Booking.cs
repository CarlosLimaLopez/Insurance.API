using Insurance.Domain.Exceptions;
using Insurance.Domain.Insuring.Enums;

namespace Insurance.Domain.Insuring.Entities;

/// <summary>
/// Represents a hotel booking that can be insured.
/// </summary>
public class Booking
{
    /// <summary>
    /// Unique booking reference.
    /// </summary>
    public string Reference { get; private set; }

    /// <summary>
    /// Check-in date of the booking.
    /// </summary>
    public DateOnly CheckIn { get; private set; }

    /// <summary>
    /// Check-out date of the booking.
    /// </summary>
    public DateOnly CheckOut { get; private set; }

    /// <summary>
    /// Number of people included in the booking.
    /// </summary>
    public int People { get; private set; }

    /// <summary>
    /// Current status of the booking (Confirmed, Modified, Cancelled).
    /// </summary>
    public BookingStatus Status { get; private set; }

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private Booking() { }

    /// <summary>
    /// Creates a new booking and validates business rules.
    /// </summary>
    /// <param name="reference">Unique booking reference.</param>
    /// <param name="checkIn">Check-in date.</param>
    /// <param name="checkOut">Check-out date.</param>
    /// <param name="people">Number of people.</param>
    public Booking(string reference, DateOnly checkIn, DateOnly checkOut, int people)
    {
        Validate(reference, checkIn, checkOut, people);

        Reference = reference;
        CheckIn = checkIn;
        CheckOut = checkOut;
        People = people;
        Status = BookingStatus.Confirmed;
    }

    /// <summary>
    /// Modifies the booking data and updates the status.
    /// </summary>
    /// <param name="newCheckIn">New check-in date.</param>
    /// <param name="newCheckOut">New check-out date.</param>
    /// <param name="newPeople">New number of people.</param>
    public void Modify(DateOnly newCheckIn, DateOnly newCheckOut, int newPeople)
    {
        Validate(Reference, newCheckIn, newCheckOut, newPeople);

        CheckIn = newCheckIn;
        CheckOut = newCheckOut;
        People = newPeople;
        Status = BookingStatus.Modified;
    }

    /// <summary>
    /// Cancels the booking if it is not already cancelled.
    /// </summary>
    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new DomainException("The booking is already cancelled.");

        Status = BookingStatus.Cancelled;
    }

    /// <summary>
    /// Calculates the number of nights for the booking.
    /// </summary>
    /// <returns>Number of nights.</returns>
    public int GetNights()
    {
        return (CheckOut.ToDateTime(TimeOnly.MinValue) - CheckIn.ToDateTime(TimeOnly.MinValue)).Days;
    }

    /// <summary>
    /// Validates business rules for a booking.
    /// </summary>
    private static void Validate(string reference, DateOnly checkIn, DateOnly checkOut, int people)
    {
        if (string.IsNullOrWhiteSpace(reference))
            throw new DomainException("Booking reference cannot be null or empty.");

        if (checkIn >= checkOut)
            throw new DomainException("Check-out date must be after check-in date.");

        if (people <= 0)
            throw new DomainException("A booking must include at least one person.");
    }
}