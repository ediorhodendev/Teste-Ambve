using Ambev.DeveloperEvaluation.Application.Customers.Queries.GetAllCustomers;
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Customers;

public sealed class GetAllCustomersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnList()
    {
        var repo = Substitute.For<ICustomerRepository>();

        var list = new List<Customer>
        {
            new("B", "2", "b@b.com", "9"),
            new("A", "1", "a@a.com", "9")
        };

        repo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(list);

        var handler = new GetAllCustomersQueryHandler(repo);

        var result = await handler.Handle(new GetAllCustomersQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Name == "A");
        Assert.Contains(result, x => x.Name == "B");
    }
}
