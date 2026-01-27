using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSaleById;

public sealed class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto>
{
    private readonly ISaleRepository _repo;
    private readonly IMapper _mapper;

    public GetSaleByIdQueryHandler(ISaleRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<SaleDto> Handle(GetSaleByIdQuery request, CancellationToken ct)
    {
        var sale = await _repo.GetByIdWithItemsAsync(request.Id, ct);
        if (sale is null)
            throw new SalesDomainException(SalesErrorMessages.SaleNotFound);

        return _mapper.Map<SaleDto>(sale);
    }
}
