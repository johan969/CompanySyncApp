using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar nya score contributions.
/// </summary>
public sealed class CreateScoreContributionRequestValidator : AbstractValidator<CreateScoreContributionRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för score contributions.
    /// </summary>
    public CreateScoreContributionRequestValidator()
    {
        RuleFor(x => x.RequirementEvaluationId).NotEqual(Guid.Empty).When(x => x.RequirementEvaluationId.HasValue);
        RuleFor(x => x.Category).MaximumLength(255);
        RuleFor(x => x.Explanation).MaximumLength(4096);
    }
}