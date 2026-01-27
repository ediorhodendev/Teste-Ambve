using Ambev.DeveloperEvaluation.Application.Branches.Dtos;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Branches;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.Commands.UpdateBranch;

public sealed class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, BranchDto>
{
    private readonly IBranchRepository _repo;

    public UpdateBranchCommandHandler(IBranchRepository repo) => _repo = repo;

    public async Task<BranchDto> Handle(UpdateBranchCommand request, CancellationToken ct)
    {
        var branch = await _repo.GetByIdTrackedAsync(request.Id, ct);
        if (branch is null)
            throw new SalesDomainException("Filial não encontrada.");

        if (await _repo.ExistsByNameAsync(request.Name, request.Id, ct))
            throw new SalesDomainException("Já existe outra filial com este nome.");

        branch.Update(request.Name, request.Location, request.IsActive);
        await _repo.UpdateAsync(branch, ct);

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
