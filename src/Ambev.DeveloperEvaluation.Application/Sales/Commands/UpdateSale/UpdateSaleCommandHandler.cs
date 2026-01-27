using Ambev.DeveloperEvaluation.Application.Sales.Abstractions;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Branches;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using Ambev.DeveloperEvaluation.Domain.Repositories.Products;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;

public sealed class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, SaleDto>
{
    private readonly ISaleRepository _saleRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly IProductRepository _productRepo;
    private readonly ISaleAuditStore _audit;
    private readonly IMapper _mapper;

    public UpdateSaleCommandHandler(
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

    public async Task<SaleDto> Handle(UpdateSaleCommand request, CancellationToken ct)
    {
        // 1) Venda
        var sale = await _saleRepo.GetByIdAsync(request.Id, ct);
        if (sale is null)
            throw new SalesDomainException(SalesErrorMessages.SaleNotFound);

        var r = request.Request;

        if (r.Items is null || r.Items.Count == 0)
            throw new SalesDomainException("A venda deve possuir ao menos 1 item.");

        // 2) Customer (snapshot)
        var customer = await _customerRepo.GetByIdAsync(r.CustomerId, ct);
        if (customer is null) throw new SalesDomainException("Cliente não encontrado.");
        if (!customer.IsActive) throw new SalesDomainException("Cliente inativo.");

        // 3) Branch (snapshot)
        var branch = await _branchRepo.GetByIdAsync(r.BranchId, ct);
        if (branch is null) throw new SalesDomainException("Filial não encontrada.");
        if (!branch.IsActive) throw new SalesDomainException("Filial inativa.");

        // 4) Products (validar + pegar Nome + Preço do banco)
        // cache: ProductId -> (Name, Price)
        var productCache = new Dictionary<Guid, (string Name, decimal Price)>();

        foreach (var item in r.Items)
        {
            if (item.Quantity <= 0)
                throw new SalesDomainException("Quantidade deve ser maior que zero.");

            if (productCache.ContainsKey(item.ProductId))
                continue;

            var product = await _productRepo.GetByIdAsync(item.ProductId, ct);
            if (product is null)
                throw new SalesDomainException($"Produto não encontrado: {item.ProductId}.");

            if (!product.IsActive)
                throw new SalesDomainException($"Produto inativo: {product.Name}.");

            // IMPORTANTE: Price vem do Product
            productCache[item.ProductId] = (product.Name, product.Price);
        }

        // 5) Atualiza header com snapshot real
        sale.UpdateHeader(
            r.SaleDate,
            customer.Id,
            customer.Name,
            branch.Id,
            branch.Name);

        // 6) Sync items (UnitPrice vem do Product.Price)
        var incoming = r.Items.Select(i =>
        {
            var p = productCache[i.ProductId];

            return new SaleItemSyncData(
                Id: i.Id,
                ProductId: i.ProductId,
                ProductName: p.Name,
                Quantity: i.Quantity,
                UnitPrice: p.Price
            );
        });

        sale.SyncItems(incoming);

        await _saleRepo.UpdateAsync(sale, ct);

        // 7) Audit
        await _audit.AppendAsync("SaleUpdated", sale.Id, new
        {
            sale.Id,
            sale.SaleNumber,
            sale.SaleDate,
            sale.CustomerId,
            sale.CustomerName,
            sale.BranchId,
            sale.BranchName,
            sale.Status,
            sale.TotalAmount,
            sale.TotalDiscount,
            Items = sale.Items.Select(i => new
            {
                i.Id,
                i.ProductId,
                i.ProductName,
                i.Quantity,
                i.UnitPrice,
                i.DiscountPercent,
                i.DiscountValue,
                i.TotalAmount,
                i.Status
            })
        }, ct);

        return _mapper.Map<SaleDto>(sale);
    }
}
