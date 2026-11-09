using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar intern approval av buyer acceptance.
/// </summary>
public sealed class ApproveBuyerProposalAcceptanceRequestValidator : AbstractValidator<ApproveBuyerProposalAcceptanceRequest>
{
    /// <summary>
    /// Konfigurerar audit- och concurrency-regler.
    /// </summary>
    public ApproveBuyerProposalAcceptanceRequestValidator()
    {
        RuleFor(x => x.InternallyApprovedByUserId).NotEqual(Guid.Empty).When(x => x.InternallyApprovedByUserId.HasValue);
        RuleFor(x => x.InternalApprovalNotes).MaximumLength(4096);
        RuleFor(x => x.BuyerProposalAcceptanceRowVersion).NotEqual(0u);
        RuleFor(x => x.BuyerProposalRowVersion).NotEqual(0u);
        RuleFor(x => x.BuyerProposalRevisionRowVersion).NotEqual(0u);
        RuleFor(x => x.BuyerProposalOptionRowVersion).NotEqual(0u);
    }
}