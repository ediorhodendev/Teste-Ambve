using Ambev.DeveloperEvaluation.Application.Common.Validation.Rules;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Branches.Commands.UpdateBranch;

public sealed class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmptyGuid("Id da filial é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmptyTrimmed("Nome da filial é obrigatório.")
            .Length(3, 200).WithMessage("Nome da filial deve ter entre 3 e 200 caracteres.");

        RuleFor(x => x.Location)
            .NotEmptyTrimmed("Localização é obrigatória.")
            .Length(2, 200).WithMessage("Localização deve ter entre 2 e 200 caracteres.");
    }
}
