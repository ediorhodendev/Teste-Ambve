using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSaleItem;

public sealed record CancelSaleItemCommand(Guid SaleId, Guid ItemId)
    : IRequest<CancelSaleItemResult>;
