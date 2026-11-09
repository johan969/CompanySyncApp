
using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar completion av matching runs.
/// </summary>
public sealed class CompleteMatchingRunRequestValidator : AbstractValidator<CompleteMatchingRunRequest>
{
    /// <summary>
    /// Konfigurerar concurrency-regeln.
    /// </summary>
    public CompleteMatchingRunRequestValidator()
    {
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}