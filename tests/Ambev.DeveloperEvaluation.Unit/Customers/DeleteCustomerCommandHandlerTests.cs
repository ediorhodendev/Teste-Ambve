using Ambev.DeveloperEvaluation.Application.Customers.Commands.DeleteCustomer;
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Customers;

public sealed class DeleteCustomerCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDelete_WhenFound()
    {
        var repo = Substitute.For<ICustomerRepository>();
        var customer = new Customer("Nome", "111", "a@a.com", "999");

        repo.GetByIdTrackedAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var handler = new DeleteCustomerCommandHandler(repo);

        await handler.Handle(new DeleteCustomerCommand(customer.Id), CancellationToken.None);

        await repo.Received(1).DeleteAsync(customer, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNotFound()
    {
        var repo = Substitute.For<ICustomerRepository>();
        repo.GetByIdTrackedAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        var handler = new DeleteCustomerCommandHandler(repo);

        await Assert.ThrowsAsync<SalesDomainException>(
            () => handler.Handle(new DeleteCustomerCommand(Guid.NewGuid()), CancellationToken.None));

        await repo.DidNotReceive().DeleteAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
    }
}
