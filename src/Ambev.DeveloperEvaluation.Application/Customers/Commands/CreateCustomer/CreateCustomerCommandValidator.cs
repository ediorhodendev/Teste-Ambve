using Ambev.DeveloperEvaluation.Application.Common.Validation.Documents;
using Ambev.DeveloperEvaluation.Application.Common.Validation.Phone;
using Ambev.DeveloperEvaluation.Application.Common.Validation.Rules;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmptyTrimmed("Nome é obrigatório.", maxLength: 200);

        RuleFor(x => x.Document)
            .NotEmptyTrimmed("Documento é obrigatório.", maxLength: 14)
            .Must(BrDocumentValidator.IsValid)
            .WithMessage("Documento inválido. Informe CPF (11) ou CNPJ (14) com apenas números.");

        RuleFor(x => x.Email)
            .NotEmptyTrimmed("E-mail é obrigatório.", maxLength: 200)
            .EmailAddress().WithMessage("E-mail inválido.");

        RuleFor(x => x.Phone)
            .NotEmptyTrimmed("Telefone é obrigatório.", maxLength: 20)
            .Must(BrPhoneValidator.IsValid)
            .WithMessage("Telefone inválido (use DDD + número, somente dígitos).");
    }
}
