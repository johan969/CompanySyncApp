using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar append-only moderation-beslut.
/// </summary>
public sealed class CreateMatchingModerationDecisionRequestValidator : AbstractValidator<CreateMatchingModerationDecisionRequest>
{
    /// <summary>
    /// Konfigurerar strukturella regler för moderation och manuella overrides.
    /// </summary>
    public CreateMatchingModerationDecisionRequestValidator()
    {
        RuleFor(x => x.Decision).IsInEnum();
        RuleFor(x => x.DecidedByUserId).NotEmpty();
        RuleFor(x => x.OverriddenOutcome).IsInEnum().When(x => x.OverriddenOutcome.HasValue);
        RuleFor(x => x.OverriddenScore).GreaterThanOrEqualTo(0m).When(x => x.OverriddenScore.HasValue);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(4096);
        RuleFor(x => x.Notes).MaximumLength(4096);
        RuleFor(x => x.SupplySolutionCandidateRowVersion).NotEqual(0u);

        RuleFor(x => x)
            .Must(HaveValidOverride)
            .WithMessage("Override values must be null unless IsManualOverride is true, and a manual override must override outcome or score.");
    }

    /// <summary>
    /// Kontrollerar att override-flaggan och override-värdena beskriver samma beslut.
    /// </summary>
    private static bool HaveValidOverride(CreateMatchingModerationDecisionRequest request)
    {
        if (!request.IsManualOverride)
        {
            return !request.OverriddenOutcome.HasValue
                && !request.OverriddenScore.HasValue;
        }

        return request.OverriddenOutcome.HasValue
            || request.OverriddenScore.HasValue;
    }
}