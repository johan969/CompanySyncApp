using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar nya requirement evaluations.
/// </summary>
public sealed class CreateRequirementEvaluationRequestValidator : AbstractValidator<CreateRequirementEvaluationRequest>
{
    /// <summary>
    /// Konfigurerar strukturella regler för requirement evaluations.
    /// </summary>
    public CreateRequirementEvaluationRequestValidator()
    {
        RuleFor(x => x.BuyerRfqRequirementId).NotEmpty();
        RuleFor(x => x.SupplySolutionLineId).NotEqual(Guid.Empty).When(x => x.SupplySolutionLineId.HasValue);
        RuleFor(x => x.SupplyAllocationId).NotEqual(Guid.Empty).When(x => x.SupplyAllocationId.HasValue);
        RuleFor(x => x.SupplySolutionStepId).NotEqual(Guid.Empty).When(x => x.SupplySolutionStepId.HasValue);
        RuleFor(x => x.Outcome).IsInEnum();
        RuleFor(x => x.VerificationState).IsInEnum();
        RuleFor(x => x.UnitOfMeasureId).GreaterThan(0).When(x => x.UnitOfMeasureId.HasValue);
        RuleFor(x => x.ActualTextValue).MaximumLength(2048);
        RuleFor(x => x.Explanation).MaximumLength(4096);

        RuleFor(x => x.EvidenceSnapshotJson)
            .Must(MatchingValidationRules.BeValidJson)
            .When(x => !string.IsNullOrWhiteSpace(x.EvidenceSnapshotJson))
            .WithMessage("EvidenceSnapshotJson must contain valid JSON.");

        RuleFor(x => x.RuleTraceJson)
            .Must(MatchingValidationRules.BeValidJson)
            .When(x => !string.IsNullOrWhiteSpace(x.RuleTraceJson))
            .WithMessage("RuleTraceJson must contain valid JSON.");

        RuleFor(x => x.SupplySolutionLineId)
            .NotNull()
            .When(x => x.SupplyAllocationId.HasValue)
            .WithMessage("SupplySolutionLineId is required when SupplyAllocationId is supplied.");
    }
}