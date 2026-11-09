using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar att manuell granskning kan startas med en giltig concurrency-token.
/// </summary>
public sealed class BeginSupplySolutionCandidateReviewRequestValidator : AbstractValidator<BeginSupplySolutionCandidateReviewRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för att starta candidate review.
    /// </summary>
    public BeginSupplySolutionCandidateReviewRequestValidator()
    {
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}