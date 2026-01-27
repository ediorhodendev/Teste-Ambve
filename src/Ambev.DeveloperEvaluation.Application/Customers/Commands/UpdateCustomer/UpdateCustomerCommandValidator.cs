using Ambev.DeveloperEvaluation.Application.Common.Validation.Documents;
using Ambev.DeveloperEvaluation.Application.Common.Validation.Phone;
using Ambev.DeveloperEvaluation.Application.Common.Validation.Rules;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmptyGuid("Id do cliente é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmptyTrimmed("Nome do cliente é obrigatório.")
            .Length(3, 200).WithMessage("Nome do cliente deve ter entre 3 e 200 caracteres.");

        RuleFor(x => x.Document)
            .NotEmptyTrimmed("Documento é obrigatório.")
            .Must(BrDocumentValidator.IsValid)
            .WithMessage("Documento inválido. Informe CPF (11) ou CNPJ (14) com apenas números.");

        RuleFor(x => x.Email)
            .NotEmptyTrimmed("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(200).WithMessage("E-mail deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Phone)
            .NotEmptyTrimmed("Telefone é obrigatório.")
            .Must(BrPhoneValidator.IsValid)
            .WithMessage("Telefone inválido. Use DDD + número (somente dígitos).");
    }
}
