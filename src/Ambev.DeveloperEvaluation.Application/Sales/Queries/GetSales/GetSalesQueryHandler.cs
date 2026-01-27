using Ambev.DeveloperEvaluation.Application.Common.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSales;

public sealed class GetSalesQueryHandler
    : IRequestHandler<GetSalesQuery, PagedResultDto<SaleDto>>
{
    private readonly ISaleRepository _repo;
    private readonly IMapper _mapper;

    public GetSalesQueryHandler(ISaleRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<SaleDto>> Handle(GetSalesQuery request, CancellationToken ct)
    {
        var f = request.Filter;

        var (items, total) = await _repo.GetPagedAsync(
            f.Page,
            f.PageSize,
            f.Status,
            f.From,
            f.To,
            f.SortBy,
            f.SortDir,
            ct);

        return new PagedResultDto<SaleDto>
        {
            Page = f.Page,
            PageSize = f.PageSize,
            Total = total,
            Items = items.Select(_mapper.Map<SaleDto>).ToList()
        };
    }
}
