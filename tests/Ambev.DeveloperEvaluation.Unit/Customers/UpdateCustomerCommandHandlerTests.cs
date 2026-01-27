using Ambev.DeveloperEvaluation.Application.Customers.Commands.UpdateCustomer;
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using Bogus;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Customers;

public sealed class UpdateCustomerCommandHandlerTests
{
    private static readonly Faker Faker = new("pt_BR");

    [Fact]
    public async Task Handle_ShouldUpdateCustomer_WhenFoundAndUnique()
    {
        // arrange
        var repo = Substitute.For<ICustomerRepository>();

        var customer = new Customer("Nome", "111", "a@a.com", "999")
        {
            // Id já é gerado internamente
        };

        repo.GetByIdTrackedAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(customer);

        repo.ExistsByDocumentAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(false);

        repo.ExistsByEmailAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var handler = new UpdateCustomerCommandHandler(repo);

        var cmd = new UpdateCustomerCommand(
            Id: customer.Id,
            Name: Faker.Person.FullName,
            Document: Faker.Random.ReplaceNumbers("###########"),
            Email: Faker.Internet.Email(),
            Phone: Faker.Phone.PhoneNumber(),
            IsActive: false);

        // act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // assert
        Assert.Equal(customer.Id, result.Id);
        Assert.Equal(cmd.Name.Trim(), result.Name);
        Assert.Equal(cmd.Document.Trim(), result.Document);
        Assert.Equal(cmd.Email.Trim(), result.Email);
        Assert.Equal(cmd.Phone.Trim(), result.Phone);
        Assert.False(result.IsActive);

        await repo.Received(1).UpdateAsync(customer, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCustomerNotFound()
    {
        var repo = Substitute.For<ICustomerRepository>();
        repo.GetByIdTrackedAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        var handler = new UpdateCustomerCommandHandler(repo);

        var cmd = new UpdateCustomerCommand(
            Id: Guid.NewGuid(),
            Name: "x",
            Document: "y",
            Email: "z@z.com",
            Phone: "1",
            IsActive: true);

        await Assert.ThrowsAsync<SalesDomainException>(() => handler.Handle(cmd, CancellationToken.None));

        await repo.DidNotReceive().UpdateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDocumentDuplicated()
    {
        var repo = Substitute.For<ICustomerRepository>();
        var customer = new Customer("Nome", "111", "a@a.com", "999");

        repo.GetByIdTrackedAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        repo.ExistsByDocumentAsync(Arg.Any<string>(), customer.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        var handler = new UpdateCustomerCommandHandler(repo);

        var cmd = new UpdateCustomerCommand(customer.Id, "novo", "duplicado", "novo@x.com", "9", true);

        await Assert.ThrowsAsync<SalesDomainException>(() => handler.Handle(cmd, CancellationToken.None));
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenEmailDuplicated()
    {
        var repo = Substitute.For<ICustomerRepository>();
        var customer = new Customer("Nome", "111", "a@a.com", "999");

        repo.GetByIdTrackedAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        repo.ExistsByDocumentAsync(Arg.Any<string>(), customer.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        repo.ExistsByEmailAsync(Arg.Any<string>(), customer.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        var handler = new UpdateCustomerCommandHandler(repo);

        var cmd = new UpdateCustomerCommand(customer.Id, "novo", "ok", "duplicado@x.com", "9", true);

        await Assert.ThrowsAsync<SalesDomainException>(() => handler.Handle(cmd, CancellationToken.None));
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
    }
}
