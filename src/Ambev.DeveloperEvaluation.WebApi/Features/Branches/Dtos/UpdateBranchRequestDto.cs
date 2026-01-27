namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches.Dtos;

public sealed class UpdateBranchRequestDto
{
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}
