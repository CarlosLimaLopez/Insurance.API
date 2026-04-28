namespace Insurance.API.Contracts.Requests;

public record BookingNotificationRequest(
    string Reference,
    string Action,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int People);