using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar kandidatens maskinella evaluation-sammanfattning.
/// </summary>
public sealed class SetCandidateEvaluationSummaryRequestValidator : AbstractValidator<SetCandidateEvaluationSummaryRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för kandidatens sammanfattade resultat.
    /// </summary>
    public SetCandidateEvaluationSummaryRequestValidator()
    {
        RuleFor(x => x.Outcome).IsInEnum();
        RuleFor(x => x.VerificationState).IsInEnum();
        RuleFor(x => x.TotalScore).GreaterThanOrEqualTo(0m);
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}