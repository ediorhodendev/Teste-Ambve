using System;
using Bogus;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales;

public sealed class SaleItemDiscountRulesTests
{
    private static readonly Faker Faker = new("pt_BR");

    private static SaleItem CreateItem(int qty, decimal unitPrice = 100m)
        => new(
            saleId: Guid.NewGuid(),
            productId: Guid.NewGuid(),
            productName: Faker.Commerce.ProductName(),
            quantity: qty,
            unitPrice: unitPrice);

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Quantity_1_to_3_ShouldHaveNoDiscount(int qty)
    {
        var item = CreateItem(qty, 100m);

        Assert.Equal(0m, item.DiscountPercent);
        Assert.Equal(0m, item.DiscountValue);
        Assert.Equal(qty * 100m, item.TotalAmount);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(9)]
    public void Quantity_4_to_9_ShouldApply10PercentDiscount(int qty)
    {
        var unit = 100m;
        var item = CreateItem(qty, unit);

        var gross = qty * unit;
        var expectedDiscount = Math.Round(gross * 0.10m, 2, MidpointRounding.AwayFromZero);
        var expectedTotal = Math.Round(gross - expectedDiscount, 2, MidpointRounding.AwayFromZero);

        Assert.Equal(0.10m, item.DiscountPercent);
        Assert.Equal(expectedDiscount, item.DiscountValue);
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(20)]
    public void Quantity_10_to_20_ShouldApply20PercentDiscount(int qty)
    {
        var unit = 150m;
        var item = CreateItem(qty, unit);

        var gross = qty * unit;
        var expectedDiscount = Math.Round(gross * 0.20m, 2, MidpointRounding.AwayFromZero);
        var expectedTotal = Math.Round(gross - expectedDiscount, 2, MidpointRounding.AwayFromZero);

        Assert.Equal(0.20m, item.DiscountPercent);
        Assert.Equal(expectedDiscount, item.DiscountValue);
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    [Theory]
    [InlineData(21)]
    [InlineData(99)]
    public void Quantity_Above_20_ShouldThrow(int qty)
    {
        var ex = Assert.Throws<SalesDomainException>(() => CreateItem(qty, 100m));
        Assert.Contains("20", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Quantity_Zero_ShouldThrow()
    {
        var ex = Assert.Throws<SalesDomainException>(() => CreateItem(0, 100m));
        Assert.Contains("Quantidade", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
