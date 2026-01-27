using Ambev.DeveloperEvaluation.Application.Branches.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.Commands.UpdateBranch;

public sealed record UpdateBranchCommand(Guid Id, string Name, string Location, bool IsActive) : IRequest<BranchDto>;
