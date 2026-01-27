namespace Ambev.DeveloperEvaluation.Application.Sales.Dtos;

public sealed class CreateSaleRequestDto
{
    public string SaleNumber { get; set; } = default!;
    public DateTime SaleDate { get; set; }

    public Guid CustomerId { get; set; }
    public Guid BranchId { get; set; }

    public List<CreateSaleItemDto> Items { get; set; } = new();
}

public sealed class CreateSaleItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
