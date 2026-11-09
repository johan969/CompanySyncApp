using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar skapande av buyer proposals.
/// </summary>
public sealed class CreateBuyerProposalRequestValidator : AbstractValidator<CreateBuyerProposalRequest>
{
    /// <summary>
    /// Konfigurerar strukturella regler för proposal och revision 1.
    /// </summary>
    public CreateBuyerProposalRequestValidator()
    {
        RuleFor(x => x.BuyerRfqRevisionId).NotEmpty();
        RuleFor(x => x.ProposalNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);
        RuleFor(x => x.CurrencyId).GreaterThan(0);
        RuleFor(x => x.CommercialTerms).MaximumLength(8000);
        RuleFor(x => x.BuyerFacingNotes).MaximumLength(8000);
        RuleFor(x => x.RevisionInternalNotes).MaximumLength(4096);
        RuleFor(x => x.DisclosurePackageRevisionId).NotEqual(Guid.Empty).When(x => x.DisclosurePackageRevisionId.HasValue);

        RuleFor(x => x)
            .Must(x =>
                !x.ValidFromUtc.HasValue
                || !x.ValidToUtc.HasValue
                || x.ValidToUtc.Value >= x.ValidFromUtc.Value)
            .WithMessage("ValidToUtc cannot be earlier than ValidFromUtc.");
    }
}