using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Common.Validation.Rules;

public static class RuleExtensions
{
    // ✅ string obrigatório (não aceita null)
    public static IRuleBuilderOptions<T, string> NotEmptyTrimmed<T>(
        this IRuleBuilderInitial<T, string> rule,
        string message,
        int maxLength = 200)
        => rule
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(message)
            .Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(message)
            .MaximumLength(maxLength).WithMessage($"Máximo de {maxLength} caracteres.");

    // ✅ string? opcional (se vier preenchido, valida trim + tamanho)
    public static IRuleBuilderOptions<T, string?> TrimmedMaxLengthIfNotNull<T>(
        this IRuleBuilderInitial<T, string?> rule,
        int maxLength,
        string messageIfTooLong)
        => rule
            .Cascade(CascadeMode.Stop)
            .Must(s => s is null || s.Trim().Length <= maxLength)
            .WithMessage(messageIfTooLong);

    // ✅ Guid obrigatório
    public static IRuleBuilderOptions<T, Guid> NotEmptyGuid<T>(
        this IRuleBuilderInitial<T, Guid> rule,
        string message)
        => rule
            .Must(g => g != Guid.Empty)
            .WithMessage(message);

    // ✅ Guid? obrigatório (nullable)
    public static IRuleBuilderOptions<T, Guid?> NotEmptyGuid<T>(
        this IRuleBuilderInitial<T, Guid?> rule,
        string message)
        => rule
            .Must(g => g.HasValue && g.Value != Guid.Empty)
            .WithMessage(message);
}
