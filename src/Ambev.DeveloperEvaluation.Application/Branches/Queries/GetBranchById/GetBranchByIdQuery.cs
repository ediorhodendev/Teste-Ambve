using Ambev.DeveloperEvaluation.Application.Branches.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.Queries.GetBranchById;

public sealed record GetBranchByIdQuery(Guid Id) : IRequest<BranchDto>;
