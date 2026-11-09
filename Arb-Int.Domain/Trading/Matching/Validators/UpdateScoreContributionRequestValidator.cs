using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar uppdateringar av score contributions.
/// </summary>
public sealed class UpdateScoreContributionRequestValidator : AbstractValidator<UpdateScoreContributionRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för score-uppdateringar.
    /// </summary>
    public UpdateScoreContributionRequestValidator()
    {
        RuleFor(x => x.Category).MaximumLength(255);
        RuleFor(x => x.Explanation).MaximumLength(4096);
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}