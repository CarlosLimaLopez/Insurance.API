namespace Insurance.Application.Insuring.Messages;

public record IssueInsurancePolicyMessage(
    string BookingReference,
    string CheckIn,
    string CheckOut,
    int People,
    decimal PremiumAmount,
    string Currency,
    string Product
);