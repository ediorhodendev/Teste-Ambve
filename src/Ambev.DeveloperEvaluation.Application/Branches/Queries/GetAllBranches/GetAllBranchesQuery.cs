using Ambev.DeveloperEvaluation.Application.Branches.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.Queries.GetAllBranches;

public sealed record GetAllBranchesQuery() : IRequest<List<BranchDto>>;
