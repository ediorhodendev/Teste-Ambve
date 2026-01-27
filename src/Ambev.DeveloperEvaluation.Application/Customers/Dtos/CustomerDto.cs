namespace Ambev.DeveloperEvaluation.Application.Customers.Dtos;

public sealed class CustomerDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = default!;
    public string Document { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Phone { get; init; } = default!;

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
