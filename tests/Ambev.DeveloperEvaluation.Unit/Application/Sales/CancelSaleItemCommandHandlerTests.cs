using Ambev.DeveloperEvaluation.Application.Sales.Abstractions;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Bogus;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public sealed class CancelSaleItemCommandHandlerTests
{
    private static readonly Faker Faker = new("pt_BR");

    private static Sale NewSaleWithTwoItems()
    {
        var s = new Sale(
            saleNumber: Faker.Random.AlphaNumeric(10),
            saleDate: DateTime.UtcNow,
            customerId: Guid.NewGuid(),
            customerName: Faker.Person.FullName,
            branchId: Guid.NewGuid(),
            branchName: Faker.Company.CompanyName());

        s.AddItem(Guid.NewGuid(), Faker.Commerce.ProductName(), 4, 100m);
        s.AddItem(Guid.NewGuid(), Faker.Commerce.ProductName(), 10, 100m);
        return s;
    }

    [Fact]
    public async Task Handle_ShouldCancelItem_UpdateRepo_AndWriteAudit()
    {
        var repo = Substitute.For<ISaleRepository>();
        var audit = Substitute.For<ISaleAuditStore>();

        var sale = NewSaleWithTwoItems();
        var itemId = sale.Items.First().Id;

        repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var handler = new CancelSaleItemCommandHandler(repo, audit);

        await handler.Handle(new CancelSaleItemCommand(sale.Id, itemId), CancellationToken.None);

        Assert.Equal(SaleItemStatus.Cancelled, sale.Items.First(i => i.Id == itemId).Status);

        await repo.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await audit.Received(1).AppendAsync("ItemCancelled", sale.Id, Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenItemNotFound_ShouldThrow()
    {
        var repo = Substitute.For<ISaleRepository>();
        var audit = Substitute.For<ISaleAuditStore>();

        var sale = NewSaleWithTwoItems();
        repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var handler = new CancelSaleItemCommandHandler(repo, audit);

        await Assert.ThrowsAsync<SalesDomainException>(() =>
            handler.Handle(new CancelSaleItemCommand(sale.Id, Guid.NewGuid()), CancellationToken.None));
    }
}
