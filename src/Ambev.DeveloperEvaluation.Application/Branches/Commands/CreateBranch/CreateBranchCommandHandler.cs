using Ambev.DeveloperEvaluation.Application.Branches.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities.Branches;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Branches;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.Commands.CreateBranch;

public sealed class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, BranchDto>
{
    private readonly IBranchRepository _repo;

    public CreateBranchCommandHandler(IBranchRepository repo) => _repo = repo;

    public async Task<BranchDto> Handle(CreateBranchCommand request, CancellationToken ct)
    {
        if (await _repo.ExistsByNameAsync(request.Name, null, ct))
            throw new SalesDomainException("Já existe filial com este nome.");

        var branch = new Branch(request.Name, request.Location);
        await _repo.AddAsync(branch, ct);

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
