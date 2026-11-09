using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som släpper en RFQ-revision.
/// </summary>
public sealed class ReleaseBuyerRfqRevisionRequestValidator : AbstractValidator<ReleaseBuyerRfqRevisionRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för release.
    /// </summary>
    public ReleaseBuyerRfqRevisionRequestValidator()
    {
        RuleFor(x => x.ReleasedByUserId).NotEqual(Guid.Empty).When(x => x.ReleasedByUserId.HasValue);
        RuleFor(x => x.BuyerRfqRowVersion).NotEqual(0u);
        RuleFor(x => x.BuyerRfqRevisionRowVersion).NotEqual(0u);
    }
}