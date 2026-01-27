using Ambev.DeveloperEvaluation.Application.Sales.Abstractions;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Bogus;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public sealed class CancelSaleCommandHandlerTests
{
    private static readonly Faker Faker = new("pt_BR");

    private static Sale NewSale()
    {
        var s = new Sale(
            saleNumber: Faker.Random.AlphaNumeric(10),
            saleDate: DateTime.UtcNow,
            customerId: Guid.NewGuid(),
            customerName: Faker.Person.FullName,
            branchId: Guid.NewGuid(),
            branchName: Faker.Company.CompanyName());

        s.AddItem(Guid.NewGuid(), Faker.Commerce.ProductName(), 4, 100m);
        return s;
    }

    [Fact]
    public async Task Handle_ShouldCancelSale_UpdateRepo_AndWriteAudit()
    {
        var repo = Substitute.For<ISaleRepository>();
        var audit = Substitute.For<ISaleAuditStore>();

        var sale = NewSale();
        repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var handler = new CancelSaleCommandHandler(repo, audit);

        await handler.Handle(new CancelSaleCommand(sale.Id), CancellationToken.None);

        Assert.Equal(SaleStatus.Cancelled, sale.Status);

        await repo.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await audit.Received(1).AppendAsync("SaleCancelled", sale.Id, Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSaleNotFound_ShouldThrow()
    {
        var repo = Substitute.For<ISaleRepository>();
        var audit = Substitute.For<ISaleAuditStore>();

        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var handler = new CancelSaleCommandHandler(repo, audit);

        await Assert.ThrowsAsync<SalesDomainException>(() =>
            handler.Handle(new CancelSaleCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
