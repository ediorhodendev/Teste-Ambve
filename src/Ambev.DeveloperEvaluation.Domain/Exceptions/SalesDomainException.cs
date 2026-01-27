namespace Ambev.DeveloperEvaluation.Domain.Exceptions;

public sealed class SalesDomainException : Exception
{
    public SalesDomainException(string message) : base(message) { }
}
