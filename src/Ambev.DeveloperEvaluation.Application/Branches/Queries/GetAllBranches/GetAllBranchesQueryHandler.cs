using Ambev.DeveloperEvaluation.Application.Branches.Dtos;
using Ambev.DeveloperEvaluation.Domain.Repositories.Branches;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.Queries.GetAllBranches;

public sealed class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, List<BranchDto>>
{
    private readonly IBranchRepository _repo;

    public GetAllBranchesQueryHandler(IBranchRepository repo) => _repo = repo;

    public async Task<List<BranchDto>> Handle(GetAllBranchesQuery request, CancellationToken ct)
    {
        var list = await _repo.GetAllAsync(ct);

        return list.Select(b => new BranchDto
        {
            Id = b.Id,
            Name = b.Name,
            Location = b.Location,
            IsActive = b.IsActive,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        }).ToList();
    }
}
