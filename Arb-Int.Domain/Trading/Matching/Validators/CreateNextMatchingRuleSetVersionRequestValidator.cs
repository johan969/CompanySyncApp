using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar skapande av nästa regelversion.
/// </summary>
public sealed class CreateNextMatchingRuleSetVersionRequestValidator : AbstractValidator<CreateNextMatchingRuleSetVersionRequest>
{
    /// <summary>
    /// Konfigurerar concurrency-regeln.
    /// </summary>
    public CreateNextMatchingRuleSetVersionRequestValidator()
    {
        RuleFor(x => x.MatchingRuleSetRowVersion).NotEqual(0u);
    }
}