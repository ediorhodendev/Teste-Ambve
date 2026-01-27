namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public sealed record SaleItemSyncData(
    Guid? Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice
);
