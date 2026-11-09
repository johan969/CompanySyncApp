using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar utfärdande av en proposal-revision.
/// </summary>
public sealed class IssueBuyerProposalRevisionRequestValidator : AbstractValidator<IssueBuyerProposalRevisionRequest>
{
    /// <summary>
    /// Konfigurerar audit- och concurrency-regler.
    /// </summary>
    public IssueBuyerProposalRevisionRequestValidator()
    {
        RuleFor(x => x.IssuedByUserId).NotEqual(Guid.Empty).When(x => x.IssuedByUserId.HasValue);
        RuleFor(x => x.BuyerProposalRowVersion).NotEqual(0u);
        RuleFor(x => x.BuyerProposalRevisionRowVersion).NotEqual(0u);
    }
}