using Ambev.DeveloperEvaluation.Application.Common.Validation.Rules;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmptyGuid("Id do produto é obrigatório.");

        RuleFor(x => x.ExternalId)
            .NotEmptyTrimmed("ExternalId é obrigatório.")
            .MaximumLength(80).WithMessage("ExternalId deve ter no máximo 80 caracteres.");

        RuleFor(x => x.Name)
            .NotEmptyTrimmed("Nome do produto é obrigatório.")
            .MaximumLength(200).WithMessage("Nome do produto deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Description!)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Price)
            .GreaterThan(0m).WithMessage("Preço deve ser maior que zero.");
    }
}
