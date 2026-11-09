using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar skapande av nästa proposal-revision.
/// </summary>
public sealed class CreateNextBuyerProposalRevisionRequestValidator : AbstractValidator<CreateNextBuyerProposalRevisionRequest>
{
    /// <summary>
    /// Konfigurerar RFQ- och concurrency-regler.
    /// </summary>
    public CreateNextBuyerProposalRevisionRequestValidator()
    {
        RuleFor(x => x.BuyerRfqRevisionId).NotEmpty();
        RuleFor(x => x.BuyerProposalRowVersion).NotEqual(0u);
    }
}