namespace Ambev.DeveloperEvaluation.Application.Sales.Dtos;

public sealed class SalesFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? Status { get; set; } 
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    public string? SortBy { get; set; } = "SaleDate";
    public string? SortDir { get; set; } = "desc";
}
