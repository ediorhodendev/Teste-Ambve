using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Mappings;

public sealed class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<Sale, SaleDto>()
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));

        CreateMap<SaleItem, SaleItemDto>();
    }
}
