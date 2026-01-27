namespace Ambev.DeveloperEvaluation.Application.Sales.Dtos;

public sealed class UpdateSaleRequestDto
{
    public DateTime SaleDate { get; set; }

    public Guid CustomerId { get; set; }
    public Guid BranchId { get; set; }

    public List<UpdateSaleItemDto> Items { get; set; } = new();
}

public sealed class UpdateSaleItemDto
{
    public Guid? Id { get; set; } // null = novo item
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
