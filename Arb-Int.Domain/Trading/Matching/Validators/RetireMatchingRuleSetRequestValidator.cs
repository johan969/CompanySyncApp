using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar pensionering av ett matchningsregelverk.
/// </summary>
public sealed class RetireMatchingRuleSetRequestValidator : AbstractValidator<RetireMatchingRuleSetRequest>
{
    /// <summary>
    /// Konfigurerar concurrency-regeln.
    /// </summary>
    public RetireMatchingRuleSetRequestValidator()
    {
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}