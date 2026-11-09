using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar nya supply-solution-candidates.
/// </summary>
public sealed class CreateSupplySolutionCandidateRequestValidator : AbstractValidator<CreateSupplySolutionCandidateRequest>
{
    /// <summary>
    /// Konfigurerar strukturella regler för candidates.
    /// </summary>
    public CreateSupplySolutionCandidateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.VerificationState).IsInEnum();
        RuleFor(x => x.CoveragePercent).InclusiveBetween(0m, 100m);
        RuleFor(x => x.EstimatedCostCurrencyId).GreaterThan(0).When(x => x.EstimatedCostCurrencyId.HasValue);
    }
}