using Ambev.DeveloperEvaluation.Application.Common.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSales;

public sealed record GetSalesQuery(SalesFilterDto Filter)
    : IRequest<PagedResultDto<SaleDto>>;
