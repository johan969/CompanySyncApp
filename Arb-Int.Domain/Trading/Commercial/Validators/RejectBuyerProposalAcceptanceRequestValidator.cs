using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar intern rejection av buyer acceptance.
/// </summary>
public sealed class RejectBuyerProposalAcceptanceRequestValidator : AbstractValidator<RejectBuyerProposalAcceptanceRequest>
{
    /// <summary>
    /// Konfigurerar notes- och concurrency-regler.
    /// </summary>
    public RejectBuyerProposalAcceptanceRequestValidator()
    {
        RuleFor(x => x.InternalApprovalNotes).MaximumLength(4096);
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}