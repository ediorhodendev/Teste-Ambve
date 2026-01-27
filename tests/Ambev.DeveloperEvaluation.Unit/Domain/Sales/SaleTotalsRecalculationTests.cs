using System;
using Bogus;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales;

public sealed class SaleTotalsRecalculationTests
{
    private static readonly Faker Faker = new("pt_BR");

    private static Sale NewSale()
        => new(
            saleNumber: Faker.Random.AlphaNumeric(10),
            saleDate: DateTime.UtcNow,
            customerId: Guid.NewGuid(),
            customerName: Faker.Person.FullName,
            branchId: Guid.NewGuid(),
            branchName: Faker.Company.CompanyName());

    [Fact]
    public void AddItems_ShouldUpdateTotals()
    {
        var sale = NewSale();

        // 3 itens => 0% => 3*100 = 300
        sale.AddItem(Guid.NewGuid(), Faker.Commerce.ProductName(), 3, 100m);

        // 4 itens => 10% => gross 400, disc 40 => total 360
        sale.AddItem(Guid.NewGuid(), Faker.Commerce.ProductName(), 4, 100m);

        Assert.Equal(660m, sale.TotalAmount);
        Assert.Equal(40m, sale.TotalDiscount);
        Assert.Equal(2, sale.Items.Count);
    }

    [Fact]
    public void CancelSale_ShouldCancelItems_AndTotalsBecomeZero()
    {
        var sale = NewSale();

        sale.AddItem(Guid.NewGuid(), Faker.Commerce.ProductName(), 4, 100m);  // total 360
        sale.AddItem(Guid.NewGuid(), Faker.Commerce.ProductName(), 10, 100m); // total 800

        sale.Cancel();

        Assert.Equal(SaleStatus.Cancelled, sale.Status);
        Assert.Equal(0m, sale.TotalAmount);
        Assert.Equal(0m, sale.TotalDiscount);

        foreach (var item in sale.Items)
            Assert.Equal(SaleItemStatus.Cancelled, item.Status);
    }
}
