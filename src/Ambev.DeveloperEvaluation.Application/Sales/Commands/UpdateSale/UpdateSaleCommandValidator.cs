using Ambev.DeveloperEvaluation.Application.Common.Validation.Rules;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;

public sealed class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmptyGuid("Id da venda é obrigatório.");

        RuleFor(x => x.Request.SaleDate)
            .Must(d => d != default)
            .WithMessage("SaleDate é obrigatório.");

        RuleFor(x => x.Request.CustomerId)
            .NotEmptyGuid("CustomerId é obrigatório.");

        RuleFor(x => x.Request.BranchId)
            .NotEmptyGuid("BranchId é obrigatório.");

        RuleFor(x => x.Request.Items)
            .NotNull().WithMessage("Items é obrigatório.")
            .Must(items => items?.Count > 0).WithMessage("A venda deve possuir ao menos 1 item.");

        RuleForEach(x => x.Request.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmptyGuid("ProductId é obrigatório.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity deve ser maior que zero.")
                .LessThanOrEqualTo(20).WithMessage("Não é permitido vender mais de 20 itens iguais.");

            // Se você usa Id opcional para sync (update/cancel por ausência)
            item.RuleFor(i => i.Id)
                .Must(id => id is null || id != Guid.Empty)
                .WithMessage("Item.Id inválido.");
        });
    }
}
