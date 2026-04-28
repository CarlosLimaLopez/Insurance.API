using Insurance.Application.Insuring.Messages;
using Insurance.Infrastructure.ExternalServices.Dtos;
using Insurance.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Insurance.Infrastructure.ExternalServices;

public class IssueInsurancePolicyConsumer : IConsumer<IssueInsurancePolicyMessage>
{
    private readonly InsuranceDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;

    public IssueInsurancePolicyConsumer(
        InsuranceDbContext dbContext,
        IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
    }

    public async Task Consume(ConsumeContext<IssueInsurancePolicyMessage> context)
    {
        var message = context.Message;

        var insuredBooking = await _dbContext.InsuredBookings
            .FirstOrDefaultAsync(ib => ib.BookingReference == message.BookingReference, context.CancellationToken);

        if (insuredBooking == null || insuredBooking.IsCancelled)
            return;

        var apiRequest = new
        {
            reference = message.BookingReference,
            check_in = message.CheckIn,
            check_out = message.CheckOut,
            people = message.People,
            premium_amount = message.PremiumAmount,
            currency = message.Currency,
            product = message.Product
        };

        var client = _httpClientFactory.CreateClient("FlexibleInsuredApi");
        var response = await client.PostAsJsonAsync("/issue-policy", apiRequest, context.CancellationToken);

        response.EnsureSuccessStatusCode();

        var apiResponse = await response.Content.ReadFromJsonAsync<IssuePolicyResponse>(cancellationToken: context.CancellationToken);

        if (apiResponse is not null && !string.IsNullOrWhiteSpace(apiResponse.policy))
        {
            insuredBooking.AssignPolicy(apiResponse.policy);
            await _dbContext.SaveChangesAsync(context.CancellationToken);
        }
    }
}