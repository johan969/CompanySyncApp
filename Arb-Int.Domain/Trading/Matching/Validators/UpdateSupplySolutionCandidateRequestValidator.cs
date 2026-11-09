using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar uppdateringar av generated candidates.
/// </summary>
public sealed class UpdateSupplySolutionCandidateRequestValidator : AbstractValidator<UpdateSupplySolutionCandidateRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för candidate-uppdateringar.
    /// </summary>
    public UpdateSupplySolutionCandidateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.VerificationState).IsInEnum();
        RuleFor(x => x.CoveragePercent).InclusiveBetween(0m, 100m);
        RuleFor(x => x.EstimatedCostCurrencyId).GreaterThan(0).When(x => x.EstimatedCostCurrencyId.HasValue);
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}