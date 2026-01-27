using Ambev.DeveloperEvaluation.Application.Customers.Commands.CreateCustomer;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using Bogus;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Customers;

public sealed class CreateCustomerCommandHandlerTests
{
    private static readonly Faker Faker = new("pt_BR");

    [Fact]
    public async Task Handle_ShouldCreateCustomer_WhenUniqueDocumentAndEmail()
    {
        // arrange
        var repo = Substitute.For<ICustomerRepository>();

        repo.ExistsByDocumentAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(false);

        repo.ExistsByEmailAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(false);

        var handler = new CreateCustomerCommandHandler(repo);

        var cmd = new CreateCustomerCommand(
            Name: Faker.Person.FullName,
            Document: Faker.Random.ReplaceNumbers("###########"),
            Email: Faker.Internet.Email(),
            Phone: Faker.Phone.PhoneNumber("+55 11 9####-####"));

        var ct = CancellationToken.None;

        // act
        var result = await handler.Handle(cmd, ct);

        // assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(cmd.Name.Trim(), result.Name);
        Assert.Equal(cmd.Document.Trim(), result.Document);
        Assert.Equal(cmd.Email.Trim(), result.Email);
        Assert.Equal(cmd.Phone.Trim(), result.Phone);
        Assert.True(result.IsActive);

        await repo.Received(1).AddAsync(Arg.Any<Ambev.DeveloperEvaluation.Domain.Entities.Customers.Customer>(), ct);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDocumentAlreadyExists()
    {
        // arrange
        var repo = Substitute.For<ICustomerRepository>();

        repo.ExistsByDocumentAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(true);

        var handler = new CreateCustomerCommandHandler(repo);

        var cmd = new CreateCustomerCommand(
            Name: Faker.Person.FullName,
            Document: "123",
            Email: Faker.Internet.Email(),
            Phone: Faker.Phone.PhoneNumber());

        // act + assert
        await Assert.ThrowsAsync<SalesDomainException>(() => handler.Handle(cmd, CancellationToken.None));

        await repo.DidNotReceive().AddAsync(Arg.Any<Ambev.DeveloperEvaluation.Domain.Entities.Customers.Customer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenEmailAlreadyExists()
    {
        // arrange
        var repo = Substitute.For<ICustomerRepository>();

        repo.ExistsByDocumentAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(false);

        repo.ExistsByEmailAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(true);

        var handler = new CreateCustomerCommandHandler(repo);

        var cmd = new CreateCustomerCommand(
            Name: Faker.Person.FullName,
            Document: Faker.Random.ReplaceNumbers("###########"),
            Email: "teste@teste.com",
            Phone: Faker.Phone.PhoneNumber());

        // act + assert
        await Assert.ThrowsAsync<SalesDomainException>(() => handler.Handle(cmd, CancellationToken.None));

        await repo.DidNotReceive().AddAsync(Arg.Any<Ambev.DeveloperEvaluation.Domain.Entities.Customers.Customer>(), Arg.Any<CancellationToken>());
    }
}
