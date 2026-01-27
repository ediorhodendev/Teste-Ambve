namespace Ambev.DeveloperEvaluation.Application.Common.Dtos;

public sealed class PagedResultDto<T>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int Total { get; init; }

    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
}
