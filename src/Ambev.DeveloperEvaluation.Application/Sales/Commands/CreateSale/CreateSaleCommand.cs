
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;

public sealed record CreateSaleCommand(CreateSaleRequestDto Request) : IRequest<SaleDto>;
