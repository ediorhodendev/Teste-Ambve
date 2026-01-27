namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

public sealed record SaleCreatedEvent(Guid SaleId, string SaleNumber, DateTime SaleDate);
