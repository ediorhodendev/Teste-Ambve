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

namespace Ambev.DeveloperEvaluation.Unit.Sales;

public sealed class CreateSaleCommandHandlerTests
{
    private static readonly Faker Faker = new("pt_BR");

    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile(new SaleProfile()));
        return config.CreateMapper();
    }

    private static CreateSaleRequestDto MakeRequest(Guid customerId, Guid branchId, Guid productId, int qty)
        => new()
        {
            SaleNumber = Faker.Random.AlphaNumeric(10),
            SaleDate = DateTime.UtcNow,
            CustomerId = customerId,
            BranchId = branchId,
            Items = new()
            {
                new CreateSaleItemDto { ProductId = productId, Quantity = qty }
            }
        };

    private static Customer CreateCustomer(string name, bool active = true)
    {
        var c = new Customer(name, "12345678901", "cliente@teste.com", "41999990000");
        if (!active) c.Update(c.Name, c.Document, c.Email, c.Phone, false);
        return c;
    }

    private static Branch CreateBranch(string name, bool active = true)
    {
        var b = new Branch(name, "Curitiba - PR");
        if (!active) b.Update(b.Name, b.Location, false);
        return b;
    }

    private static Product CreateProduct(string externalId, string name, decimal price, bool active = true)
    {
        var p = new Product(externalId, name, "desc", price);
        if (!active) p.Update(p.ExternalId, p.Name, p.Description, p.Price, false);
        return p;
    }

    [Fact]
    public async Task Handle_ShouldPersistSale_UsingNamesFromDb_AndPriceFromProduct()
    {
        // arrange
        var saleRepo = Substitute.For<ISaleRepository>();
        var customerRepo = Substitute.For<ICustomerRepository>();
        var branchRepo = Substitute.For<IBranchRepository>();
        var productRepo = Substitute.For<IProductRepository>();
        var audit = Substitute.For<ISaleAuditStore>();
        var mapper = CreateMapper();

        var customer = CreateCustomer("Maria Oliveira");
        var branch = CreateBranch("Filial São Paulo Paulista");
        var product = CreateProduct("SKU-001", "Cerveja Lager 600ml", price: 12.50m);

        customerRepo.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        branchRepo.GetByIdAsync(branch.Id, Arg.Any<CancellationToken>()).Returns(branch);
        productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        saleRepo.ExistsBySaleNumberAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(false);

        var req = MakeRequest(customer.Id, branch.Id, product.Id, qty: 2);

        var handler = new CreateSaleCommandHandler(saleRepo, customerRepo, branchRepo, productRepo, audit, mapper);

        // act
        var result = await handler.Handle(new CreateSaleCommand(req), CancellationToken.None);

        // assert
        Assert.Equal(customer.Id, result.CustomerId);
        Assert.Equal("Maria Oliveira", result.CustomerName);
        Assert.Equal(branch.Id, result.BranchId);
        Assert.Equal("Filial São Paulo Paulista", result.BranchName);

        Assert.Single(result.Items);
        Assert.Equal(product.Id, result.Items[0].ProductId);
        Assert.Equal("Cerveja Lager 600ml", result.Items[0].ProductName);
        Assert.Equal(12.50m, result.Items[0].UnitPrice);

        await saleRepo.Received(1).AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await audit.Received(1).AppendAsync("SaleCreated", Arg.Any<Guid>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSaleNumberAlreadyExists_ShouldThrow()
    {
        var saleRepo = Substitute.For<ISaleRepository>();
        var customerRepo = Substitute.For<ICustomerRepository>();
        var branchRepo = Substitute.For<IBranchRepository>();
        var productRepo = Substitute.For<IProductRepository>();
        var audit = Substitute.For<ISaleAuditStore>();
        var mapper = CreateMapper();

        saleRepo.ExistsBySaleNumberAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(true);

        var req = MakeRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), qty: 1);
        var handler = new CreateSaleCommandHandler(saleRepo, customerRepo, branchRepo, productRepo, audit, mapper);

        await Assert.ThrowsAsync<SalesDomainException>(() => handler.Handle(new CreateSaleCommand(req), CancellationToken.None));

        await saleRepo.DidNotReceive().AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductHasZeroPrice_ShouldThrow()
    {
        var saleRepo = Substitute.For<ISaleRepository>();
        var customerRepo = Substitute.For<ICustomerRepository>();
        var branchRepo = Substitute.For<IBranchRepository>();
        var productRepo = Substitute.For<IProductRepository>();
        var audit = Substitute.For<ISaleAuditStore>();
        var mapper = CreateMapper();

        var customer = CreateCustomer("João da Silva");
        var branch = CreateBranch("Filial Curitiba Centro");
        var product = CreateProduct("SKU-001", "Cerveja Lager 600ml", price: 0m);

        customerRepo.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        branchRepo.GetByIdAsync(branch.Id, Arg.Any<CancellationToken>()).Returns(branch);
        productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        saleRepo.ExistsBySaleNumberAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(false);

        var req = MakeRequest(customer.Id, branch.Id, product.Id, qty: 1);
        var handler = new CreateSaleCommandHandler(saleRepo, customerRepo, branchRepo, productRepo, audit, mapper);

        await Assert.ThrowsAsync<SalesDomainException>(() => handler.Handle(new CreateSaleCommand(req), CancellationToken.None));
        await saleRepo.DidNotReceive().AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}
