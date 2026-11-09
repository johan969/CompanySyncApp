using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar en buyer acceptance-line.
/// </summary>
public sealed class RecordBuyerProposalAcceptanceLineRequestValidator : AbstractValidator<RecordBuyerProposalAcceptanceLineRequest>
{
    /// <summary>
    /// Konfigurerar line-, quantity- och textregler.
    /// </summary>
    public RecordBuyerProposalAcceptanceLineRequestValidator()
    {
        RuleFor(x => x.BuyerProposalLineId).NotEmpty();
        RuleFor(x => x.AcceptedQuantity).GreaterThan(0m);
        RuleFor(x => x.BuyerNotes).MaximumLength(4096);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);
    }
}