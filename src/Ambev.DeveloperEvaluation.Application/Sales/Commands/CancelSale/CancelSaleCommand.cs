using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;

public sealed record CancelSaleCommand(Guid SaleId) : IRequest<CancelSaleResponseDto>;
