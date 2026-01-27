using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;

public sealed record UpdateSaleCommand(Guid Id, UpdateSaleRequestDto Request) : IRequest<SaleDto>;
