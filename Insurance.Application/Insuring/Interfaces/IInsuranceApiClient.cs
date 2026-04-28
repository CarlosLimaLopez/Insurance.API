namespace Insurance.Application.Insuring.Interfaces;

public interface IInsuranceApiClient
{
    Task<string> IssuePolicyAsync(string bookingReference, decimal premiumAmount, CancellationToken cancellationToken = default);
}