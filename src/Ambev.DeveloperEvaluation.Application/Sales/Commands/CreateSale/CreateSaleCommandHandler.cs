using Ambev.DeveloperEvaluation.Application.Sales.Abstractions;
using Ambev.DeveloperEvaluation.Application.Sales.Audit;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Branches;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using Ambev.DeveloperEvaluation.Domain.Repositories.Products;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;

public sealed class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, SaleDto>
{
    private readonly ISaleRepository _saleRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly IProductRepository _productRepo;
    private readonly ISaleAuditStore _audit;
    private readonly IMapper _mapper;

    public CreateSaleCommandHandler(
        ISaleRepository saleRepo,
        ICustomerRepository customerRepo,
        IBranchRepository branchRepo,
        IProductRepository productRepo,
        ISaleAuditStore audit,
        IMapper mapper)
    {
        _saleRepo = saleRepo;
        _customerRepo = customerRepo;
        _branchRepo = branchRepo;
        _productRepo = productRepo;
        _audit = audit;
        _mapper = mapper;
    }

    public async Task<SaleDto> Handle(CreateSaleCommand request, CancellationToken ct)
    {
        var r = request.Request;

        // Unicidade do SaleNumber (mais profissional que “estourar” no banco)
        if (await _saleRepo.ExistsBySaleNumberAsync(r.SaleNumber.Trim(), excludingId: null, ct))
            throw new SalesDomainException("Já existe uma venda com este SaleNumber.");

        // Customer
        var customer = await _customerRepo.GetByIdAsync(r.CustomerId, ct);
        if (customer is null) throw new SalesDomainException("Cliente não encontrado.");
        if (!customer.IsActive) throw new SalesDomainException("Cliente inativo.");

        // Branch
        var branch = await _branchRepo.GetByIdAsync(r.BranchId, ct);
        if (branch is null) throw new SalesDomainException("Filial não encontrada.");
        if (!branch.IsActive) throw new SalesDomainException("Filial inativa.");

        var sale = new Sale(
            r.SaleNumber.Trim(),
            r.SaleDate,
            customer.Id,
            customer.Name,
            branch.Id,
            branch.Name
        );

        // Produtos e preço (snapshot)
        foreach (var item in r.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId, ct);
            if (product is null) throw new SalesDomainException($"Produto não encontrado: {item.ProductId}.");
            if (!product.IsActive) throw new SalesDomainException($"Produto inativo: {product.Name}.");
            if (product.Price <= 0) throw new SalesDomainException($"Produto sem preço válido: {product.Name}.");

            sale.AddItem(product.Id, product.Name, item.Quantity, product.Price);
        }

        await _saleRepo.AddAsync(sale, ct);

        // Audit (DTO serializável)
        var payload = ToAuditPayload(sale);
        await _audit.AppendAsync("SaleCreated", sale.Id, payload, ct);

        return _mapper.Map<SaleDto>(sale);
    }

    private static SaleAuditPayloadDto ToAuditPayload(Sale sale)
        => new()
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            BranchId = sale.BranchId,
            BranchName = sale.BranchName,
            Status = sale.Status,
            TotalAmount = sale.TotalAmount,
            TotalDiscount = sale.TotalDiscount,
            Items = sale.Items.Select(i => new SaleAuditItemPayloadDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                DiscountPercent = i.DiscountPercent,
                DiscountValue = i.DiscountValue,
                TotalAmount = i.TotalAmount,
                Status = i.Status
            }).ToList()
        };
}
