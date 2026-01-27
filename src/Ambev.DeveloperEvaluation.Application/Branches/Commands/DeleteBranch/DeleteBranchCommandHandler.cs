using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Branches;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.Commands.DeleteBranch;

public sealed class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand>
{
    private readonly IBranchRepository _repo;

    public DeleteBranchCommandHandler(IBranchRepository repo) => _repo = repo;

    public async Task Handle(DeleteBranchCommand request, CancellationToken ct)
    {
        var branch = await _repo.GetByIdTrackedAsync(request.Id, ct);
        if (branch is null)
            throw new SalesDomainException("Filial não encontrada.");

        await _repo.DeleteAsync(branch, ct);
    }
}
