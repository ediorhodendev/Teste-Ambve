using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSaleById;

public sealed record GetSaleByIdQuery(Guid Id) : IRequest<SaleDto>;
