using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar köning av en matchningskörning.
/// </summary>
public sealed class QueueMatchingRunRequestValidator : AbstractValidator<QueueMatchingRunRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för en ny matching run.
    /// </summary>
    public QueueMatchingRunRequestValidator()
    {
        RuleFor(x => x.MatchingRuleSetId).NotEmpty();
        RuleFor(x => x.MatchingRuleSetVersionId).NotEqual(Guid.Empty).When(x => x.MatchingRuleSetVersionId.HasValue);
        RuleFor(x => x.StartedByUserId).NotEqual(Guid.Empty).When(x => x.StartedByUserId.HasValue);
        RuleFor(x => x.TriggerName).MaximumLength(255);
        RuleFor(x => x.CorrelationId).MaximumLength(255);
    }
}