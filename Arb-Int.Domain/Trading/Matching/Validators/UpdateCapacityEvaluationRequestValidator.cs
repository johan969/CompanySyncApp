using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar uppdateringar av capacity evaluations.
/// </summary>
public sealed class UpdateCapacityEvaluationRequestValidator : AbstractValidator<UpdateCapacityEvaluationRequest>
{
    /// <summary>
    /// Konfigurerar de regler som uttrycks av den befintliga modellen.
    /// </summary>
    public UpdateCapacityEvaluationRequestValidator()
    {
        RuleFor(x => x.Outcome).IsInEnum();
        RuleFor(x => x.QuantityUnitId).GreaterThan(0);
        RuleFor(x => x.Explanation).MaximumLength(4096);
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}