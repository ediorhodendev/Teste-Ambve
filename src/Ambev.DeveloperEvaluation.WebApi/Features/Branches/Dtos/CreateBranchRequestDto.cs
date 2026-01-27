namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches.Dtos;

public sealed class CreateBranchRequestDto
{
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
}
