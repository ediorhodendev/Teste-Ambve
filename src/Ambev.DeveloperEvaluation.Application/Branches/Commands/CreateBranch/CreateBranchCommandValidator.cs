using Ambev.DeveloperEvaluation.Application.Common.Validation.Rules;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Branches.Commands.CreateBranch;

public sealed class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmptyTrimmed("Nome da filial é obrigatório.")
            .Length(3, 200).WithMessage("Nome da filial deve ter entre 3 e 200 caracteres.");

        RuleFor(x => x.Location)
            .NotEmptyTrimmed("Localização é obrigatória.")
            .Length(2, 200).WithMessage("Localização deve ter entre 2 e 200 caracteres.");
    }
}
