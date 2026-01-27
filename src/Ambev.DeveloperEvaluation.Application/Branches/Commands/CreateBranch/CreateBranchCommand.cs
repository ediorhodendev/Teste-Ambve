using Ambev.DeveloperEvaluation.Application.Branches.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.Commands.CreateBranch;

public sealed record CreateBranchCommand(string Name, string Location) : IRequest<BranchDto>;
