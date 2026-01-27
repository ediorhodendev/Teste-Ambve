namespace Ambev.DeveloperEvaluation.Application.Branches.Dtos;

public sealed class BranchDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Location { get; init; } = default!;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
