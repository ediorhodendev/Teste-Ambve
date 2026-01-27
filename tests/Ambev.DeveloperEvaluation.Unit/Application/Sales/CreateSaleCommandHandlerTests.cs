using Ambev.DeveloperEvaluation.Application.Sales.Abstractions;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities.Branches;
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
using Ambev.DeveloperEvaluation.Domain.Entities.Products;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Branches;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using Ambev.DeveloperEvaluation.Domain.Repositories.Products;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Mappings;
using AutoMapper;
using Bogus;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public sealed class CreateSaleCommandHandlerTests
{
    private static readonly Faker Faker = new("pt_BR");

    private static IMapper CreateMapper()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile(new SaleProfile()));
        return cfg.CreateMapper();
    }

    // ✅ Request “profissional”: só ids + qty (sem nomes, sem unitPrice)
    private static (CreateSaleRequestDto Request, Guid CustomerId, Guid BranchId, Guid ProductId) NewRequest(
        int quantity)
    {
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var req = new CreateSaleRequestDto
        {
            SaleNumber = Faker.Random.AlphaNumeric(10),
            SaleDate = DateTime.UtcNow,
            CustomerId = customerId,
            BranchId = branchId,
            Items =
            {
                new CreateSaleItemDto
                {
                    ProductId = productId,
                    Quantity = quantity
                }
            }
        };

        return (req, customerId, branchId, productId);
    }

    // Helpers para entidades (ajuste conforme seus construtores reais)
    private static Customer CreateCustomer(string name, bool isActive = true)
    {
        var c = new Customer(name, "12345678901", "cliente@teste.com", "48999999999");
        if (!isActive) c.Update(c.Name, c.Document, c.Email, c.Phone, false);
        return c;
    }

    private static Branch CreateBranch(string name, bool isActive = true)
    {
        var b = new Branch(name, "Florianópolis");
        if (!isActive) b.Update(b.Name, b.Location, false);
        return b;
    }

    private static Product CreateProduct(string externalId, string name, decimal price, bool isActive = true)
    {
        // ⚠️ Considerando seu erro: Product(string externalId, string name, string? desc, decimal price)
        var p = new Product(externalId, name, "desc", price);
        if (!isActive) p.Update(p.ExternalId, p.Name, p.Description, p.Price, false);
        return p;
    }

    [Fact]
    public async Task Handle_ShouldPersistSale_AndWriteAudit_UsingRealSnapshotsAndProductPrice()
    {
        // arrange
        var saleRepo = Substitute.For<ISaleRepository>();
        var audit = Substitute.For<ISaleAuditStore>();
        var customerRepo = Substitute.For<ICustomerRepository>();
        var branchRepo = Substitute.For<IBranchRepository>();
        var productRepo = Substitute.For<IProductRepository>();
        var mapper = CreateMapper();

        var (req, customerId, branchId, productId) = NewRequest(quantity: 4);

        var customer = CreateCustomer("Cliente Real", isActive: true);
        var branch = CreateBranch("Filial Real", isActive: true);
        var product = CreateProduct("SKU-001", "Produto Real", price: 100m, isActive: true);

        customerRepo.GetByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns(customer);
        branchRepo.GetByIdAsync(branchId, Arg.Any<CancellationToken>()).Returns(branch);
        productRepo.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);

        var handler = new CreateSaleCommandHandler(
            saleRepo, customerRepo, branchRepo, productRepo, audit, mapper);

        var cmd = new CreateSaleCommand(req);

        // act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(req.SaleNumber, result.SaleNumber);
        Assert.Single(result.Items);

        // ✅ snapshot real (não vem do request)
        Assert.Equal(customer.Id, result.CustomerId);
        Assert.Equal("Cliente Real", result.CustomerName);

        Assert.Equal(branch.Id, result.BranchId);
        Assert.Equal("Filial Real", result.BranchName);

        // ✅ item com nome e preço reais
        var item = result.Items[0];
        Assert.Equal(productId, item.ProductId);
        Assert.Equal("Produto Real", item.ProductName);
        Assert.Equal(100m, item.UnitPrice);

        // qty=4 => 10% (pela regra padrão)
        Assert.Equal(0.10m, item.DiscountPercent);
        Assert.Equal(40m, item.DiscountValue);
        Assert.Equal(360m, item.TotalAmount);

        await saleRepo.Received(1).AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await audit.Received(1).AppendAsync("SaleCreated", Arg.Any<Guid>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenQuantity10_ShouldApply20PercentDiscount_UsingProductPrice()
    {
        // arrange
        var saleRepo = Substitute.For<ISaleRepository>();
        var audit = Substitute.For<ISaleAuditStore>();
        var customerRepo = Substitute.For<ICustomerRepository>();
        var branchRepo = Substitute.For<IBranchRepository>();
        var productRepo = Substitute.For<IProductRepository>();
        var mapper = CreateMapper();

        var (req, customerId, branchId, productId) = NewRequest(quantity: 10);

        customerRepo.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(CreateCustomer("Cliente Real", true));
        branchRepo.GetByIdAsync(branchId, Arg.Any<CancellationToken>())
            .Returns(CreateBranch("Filial Real", true));
        productRepo.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(CreateProduct("SKU-001", "Produto Real", price: 100m, isActive: true));

        var handler = new CreateSaleCommandHandler(
            saleRepo, customerRepo, branchRepo, productRepo, audit, mapper);

        // act
        var result = await handler.Handle(new CreateSaleCommand(req), CancellationToken.None);

        // assert
        var item = result.Items[0];
        Assert.Equal(100m, item.UnitPrice);

        // qty=10 => 20%
        Assert.Equal(0.20m, item.DiscountPercent);
        Assert.Equal(200m, item.DiscountValue);
        Assert.Equal(800m, item.TotalAmount);
    }

    [Fact]
    public async Task Handle_WhenQuantityAbove20_ShouldThrow_AndNotPersistOrAudit()
    {
        // arrange
        var saleRepo = Substitute.For<ISaleRepository>();
        var audit = Substitute.For<ISaleAuditStore>();
        var customerRepo = Substitute.For<ICustomerRepository>();
        var branchRepo = Substitute.For<IBranchRepository>();
        var productRepo = Substitute.For<IProductRepository>();
        var mapper = CreateMapper();

        var (req, customerId, branchId, productId) = NewRequest(quantity: 21);

        customerRepo.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(CreateCustomer("Cliente Real", true));
        branchRepo.GetByIdAsync(branchId, Arg.Any<CancellationToken>())
            .Returns(CreateBranch("Filial Real", true));
        productRepo.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(CreateProduct("SKU-001", "Produto Real", price: 100m, isActive: true));

        var handler = new CreateSaleCommandHandler(
            saleRepo, customerRepo, branchRepo, productRepo, audit, mapper);

        // act + assert
        await Assert.ThrowsAsync<SalesDomainException>(() =>
            handler.Handle(new CreateSaleCommand(req), CancellationToken.None));

        await saleRepo.DidNotReceive().AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await audit.DidNotReceive().AppendAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCustomerNotFound_ShouldThrow_AndNotPersistOrAudit()
    {
        // arrange
        var saleRepo = Substitute.For<ISaleRepository>();
        var audit = Substitute.For<ISaleAuditStore>();
        var customerRepo = Substitute.For<ICustomerRepository>();
        var branchRepo = Substitute.For<IBranchRepository>();
        var productRepo = Substitute.For<IProductRepository>();
        var mapper = CreateMapper();

        var (req, customerId, branchId, productId) = NewRequest(quantity: 4);

        customerRepo.GetByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns((Customer?)null);
        branchRepo.GetByIdAsync(branchId, Arg.Any<CancellationToken>()).Returns(CreateBranch("Filial Real", true));
        productRepo.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(CreateProduct("SKU-001", "Produto Real", 100m, true));

        var handler = new CreateSaleCommandHandler(
            saleRepo, customerRepo, branchRepo, productRepo, audit, mapper);

        // act + assert
        await Assert.ThrowsAsync<SalesDomainException>(() =>
            handler.Handle(new CreateSaleCommand(req), CancellationToken.None));

        await saleRepo.DidNotReceive().AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await audit.DidNotReceive().AppendAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCustomerInactive_ShouldThrow_AndNotPersistOrAudit()
    {
        // arrange
        var saleRepo = Substitute.For<ISaleRepository>();
        var audit = Substitute.For<ISaleAuditStore>();
        var customerRepo = Substitute.For<ICustomerRepository>();
        var branchRepo = Substitute.For<IBranchRepository>();
        var productRepo = Substitute.For<IProductRepository>();
        var mapper = CreateMapper();

        var (req, customerId, branchId, productId) = NewRequest(quantity: 4);

        customerRepo.GetByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns(CreateCustomer("Cliente", isActive: false));
        branchRepo.GetByIdAsync(branchId, Arg.Any<CancellationToken>()).Returns(CreateBranch("Filial Real", true));
        productRepo.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(CreateProduct("SKU-001", "Produto Real", 100m, true));

        var handler = new CreateSaleCommandHandler(
            saleRepo, customerRepo, branchRepo, productRepo, audit, mapper);

        // act + assert
        await Assert.ThrowsAsync<SalesDomainException>(() =>
            handler.Handle(new CreateSaleCommand(req), CancellationToken.None));

        await saleRepo.DidNotReceive().AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await audit.DidNotReceive().AppendAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenBranchInactive_ShouldThrow_AndNotPersistOrAudit()
    {
        // arrange
        var saleRepo = Substitute.For<ISaleRepository>();
        var audit = Substitute.For<ISaleAuditStore>();
        var customerRepo = Substitute.For<ICustomerRepository>();
        var branchRepo = Substitute.For<IBranchRepository>();
        var productRepo = Substitute.For<IProductRepository>();
        var mapper = CreateMapper();

        var (req, customerId, branchId, productId) = NewRequest(quantity: 4);

        customerRepo.GetByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns(CreateCustomer("Cliente Real", true));
        branchRepo.GetByIdAsync(branchId, Arg.Any<CancellationToken>()).Returns(CreateBranch("Filial", isActive: false));
        productRepo.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(CreateProduct("SKU-001", "Produto Real", 100m, true));

        var handler = new CreateSaleCommandHandler(
            saleRepo, customerRepo, branchRepo, productRepo, audit, mapper);

        // act + assert
        await Assert.ThrowsAsync<SalesDomainException>(() =>
            handler.Handle(new CreateSaleCommand(req), CancellationToken.None));

        await saleRepo.DidNotReceive().AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await audit.DidNotReceive().AppendAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductInactive_ShouldThrow_AndNotPersistOrAudit()
    {
        // arrange
        var saleRepo = Substitute.For<ISaleRepository>();
        var audit = Substitute.For<ISaleAuditStore>();
        var customerRepo = Substitute.For<ICustomerRepository>();
        var branchRepo = Substitute.For<IBranchRepository>();
        var productRepo = Substitute.For<IProductRepository>();
        var mapper = CreateMapper();

        var (req, customerId, branchId, productId) = NewRequest(quantity: 4);

        customerRepo.GetByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns(CreateCustomer("Cliente Real", true));
        branchRepo.GetByIdAsync(branchId, Arg.Any<CancellationToken>()).Returns(CreateBranch("Filial Real", true));
        productRepo.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(CreateProduct("SKU-001", "Produto Real", 100m, isActive: false));

        var handler = new CreateSaleCommandHandler(
            saleRepo, customerRepo, branchRepo, productRepo, audit, mapper);

        // act + assert
        await Assert.ThrowsAsync<SalesDomainException>(() =>
            handler.Handle(new CreateSaleCommand(req), CancellationToken.None));

        await saleRepo.DidNotReceive().AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await audit.DidNotReceive().AppendAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenItemsEmpty_ShouldThrow_AndNotPersistOrAudit()
    {
        // arrange
        var saleRepo = Substitute.For<ISaleRepository>();
        var audit = Substitute.For<ISaleAuditStore>();
        var customerRepo = Substitute.For<ICustomerRepository>();
        var branchRepo = Substitute.For<IBranchRepository>();
        var productRepo = Substitute.For<IProductRepository>();
        var mapper = CreateMapper();

        var req = new CreateSaleRequestDto
        {
            SaleNumber = Faker.Random.AlphaNumeric(10),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new()
        };

        var handler = new CreateSaleCommandHandler(
            saleRepo, customerRepo, branchRepo, productRepo, audit, mapper);

        // act + assert
        await Assert.ThrowsAsync<SalesDomainException>(() =>
            handler.Handle(new CreateSaleCommand(req), CancellationToken.None));

        await saleRepo.DidNotReceive().AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await audit.DidNotReceive().AppendAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }
}
