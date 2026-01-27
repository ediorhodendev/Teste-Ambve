
namespace Ambev.DeveloperEvaluation.Application.Sales.Dtos;

public sealed record SaleItemSyncInput(
    Guid? Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice
);
