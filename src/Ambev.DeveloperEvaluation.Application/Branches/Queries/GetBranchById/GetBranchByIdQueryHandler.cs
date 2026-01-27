using Ambev.DeveloperEvaluation.Application.Branches.Dtos;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Branches;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.Queries.GetBranchById;

public sealed class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, BranchDto>
{
    private readonly IBranchRepository _repo;

    public GetBranchByIdQueryHandler(IBranchRepository repo) => _repo = repo;

    public async Task<BranchDto> Handle(GetBranchByIdQuery request, CancellationToken ct)
    {
        var branch = await _repo.GetByIdAsync(request.Id, ct);
        if (branch is null)
            throw new SalesDomainException("Filial não encontrada.");

        return new BranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            Location = branch.Location,
            IsActive = branch.IsActive,
            CreatedAt = branch.CreatedAt,
            UpdatedAt = branch.UpdatedAt
        };
    }
}
