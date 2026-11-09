using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar explicit expiration av en supply-solution-candidate.
/// </summary>
public sealed class ExpireSupplySolutionCandidateRequestValidator : AbstractValidator<ExpireSupplySolutionCandidateRequest>
{
    /// <summary>
    /// Konfigurerar concurrency-regeln för expiration.
    /// </summary>
    public ExpireSupplySolutionCandidateRequestValidator()
    {
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}