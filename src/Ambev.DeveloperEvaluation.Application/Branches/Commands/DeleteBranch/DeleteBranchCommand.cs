using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.Commands.DeleteBranch;

public sealed record DeleteBranchCommand(Guid Id) : IRequest;
