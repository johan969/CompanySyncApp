using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar finalisering av supply-solution-candidates.
/// </summary>
public sealed class FinalizeSupplySolutionCandidateRequestValidator : AbstractValidator<FinalizeSupplySolutionCandidateRequest>
{
    /// <summary>
    /// Konfigurerar concurrency-reglerna.
    /// </summary>
    public FinalizeSupplySolutionCandidateRequestValidator()
    {
        RuleFor(x => x.SupplySolutionCandidateRowVersion).NotEqual(0u);
        RuleFor(x => x.MatchingRunRowVersion).NotEqual(0u);
    }
}