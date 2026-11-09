using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar nya capacity evaluations.
/// </summary>
public sealed class CreateCapacityEvaluationRequestValidator : AbstractValidator<CreateCapacityEvaluationRequest>
{
    /// <summary>
    /// Konfigurerar de regler som uttrycks av den befintliga modellen.
    /// </summary>
    public CreateCapacityEvaluationRequestValidator()
    {
        RuleFor(x => x.SupplyAllocationId).NotEqual(Guid.Empty).When(x => x.SupplyAllocationId.HasValue);
        RuleFor(x => x.Outcome).IsInEnum();
        RuleFor(x => x.QuantityUnitId).GreaterThan(0);
        RuleFor(x => x.Explanation).MaximumLength(4096);
    }
}