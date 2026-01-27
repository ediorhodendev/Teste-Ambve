namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers.Dtos;

public sealed class CreateCustomerRequestDto
{
    public string Name { get; set; } = default!;
    public string Document { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
}
