namespace Insurance.Infrastructure.ExternalServices.Dtos;

public class IssuePolicyResponse
{
    public string reference { get; set; } = default!;
    public string policy { get; set; } = default!;
}