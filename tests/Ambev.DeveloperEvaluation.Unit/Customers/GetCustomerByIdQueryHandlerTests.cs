using Ambev.DeveloperEvaluation.Application.Customers.Queries.GetCustomerById;
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Customers;

public sealed class GetCustomerByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCustomer_WhenFound()
    {
        var repo = Substitute.For<ICustomerRepository>();
        var customer = new Customer("Nome", "111", "a@a.com", "999");

        repo.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var handler = new GetCustomerByIdQueryHandler(repo);

        var dto = await handler.Handle(new GetCustomerByIdQuery(customer.Id), CancellationToken.None);

        Assert.Equal(customer.Id, dto.Id);
        Assert.Equal(customer.Name, dto.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNotFound()
    {
        var repo = Substitute.For<ICustomerRepository>();
        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        var handler = new GetCustomerByIdQueryHandler(repo);

        await Assert.ThrowsAsync<SalesDomainException>(
            () => handler.Handle(new GetCustomerByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
