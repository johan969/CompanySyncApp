using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar uppdateringar av requirement evaluations.
/// </summary>
public sealed class UpdateRequirementEvaluationRequestValidator : AbstractValidator<UpdateRequirementEvaluationRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för evaluation-resultat.
    /// </summary>
    public UpdateRequirementEvaluationRequestValidator()
    {
        RuleFor(x => x.Outcome).IsInEnum();
        RuleFor(x => x.VerificationState).IsInEnum();
        RuleFor(x => x.UnitOfMeasureId).GreaterThan(0).When(x => x.UnitOfMeasureId.HasValue);
        RuleFor(x => x.ActualTextValue).MaximumLength(2048);
        RuleFor(x => x.Explanation).MaximumLength(4096);
        RuleFor(x => x.RowVersion).NotEqual(0u);

        RuleFor(x => x.EvidenceSnapshotJson)
            .Must(MatchingValidationRules.BeValidJson)
            .When(x => !string.IsNullOrWhiteSpace(x.EvidenceSnapshotJson))
            .WithMessage("EvidenceSnapshotJson must contain valid JSON.");

        RuleFor(x => x.RuleTraceJson)
            .Must(MatchingValidationRules.BeValidJson)
            .When(x => !string.IsNullOrWhiteSpace(x.RuleTraceJson))
            .WithMessage("RuleTraceJson must contain valid JSON.");
    }
}