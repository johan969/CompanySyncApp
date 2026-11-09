using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar avbrott av en matching run.
/// </summary>
public sealed class CancelMatchingRunRequestValidator : AbstractValidator<CancelMatchingRunRequest>
{
    /// <summary>
    /// Konfigurerar concurrency-regeln.
    /// </summary>
    public CancelMatchingRunRequestValidator()
    {
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}