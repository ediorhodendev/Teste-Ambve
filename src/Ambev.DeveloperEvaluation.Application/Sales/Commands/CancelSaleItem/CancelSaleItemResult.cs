namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSaleItem;

public sealed record CancelSaleItemResult
(
    Guid SaleId,
    Guid ItemId,

    decimal ItemTotalAmount,
    decimal ItemDiscount,

    decimal SaleTotalAmount,
    decimal SaleTotalDiscount
);
