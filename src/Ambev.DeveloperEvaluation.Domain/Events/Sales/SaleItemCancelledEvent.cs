namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

public sealed record SaleItemCancelledEvent(Guid SaleId, Guid ItemId);
