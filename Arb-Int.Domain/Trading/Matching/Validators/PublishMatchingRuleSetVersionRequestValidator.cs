using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar publicering av en regelversion.
/// </summary>
public sealed class PublishMatchingRuleSetVersionRequestValidator : AbstractValidator<PublishMatchingRuleSetVersionRequest>
{
    /// <summary>
    /// Konfigurerar publiceringsreglerna.
    /// </summary>
    public PublishMatchingRuleSetVersionRequestValidator()
    {
        RuleFor(x => x.PublishedByUserId).NotEqual(Guid.Empty).When(x => x.PublishedByUserId.HasValue);
        RuleFor(x => x.MatchingRuleSetRowVersion).NotEqual(0u);
        RuleFor(x => x.MatchingRuleSetVersionRowVersion).NotEqual(0u);
    }
}