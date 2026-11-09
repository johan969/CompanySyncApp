using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar uppdatering av Draft proposal-revisions.
/// </summary>
public sealed class UpdateBuyerProposalRevisionRequestValidator : AbstractValidator<UpdateBuyerProposalRevisionRequest>
{
    /// <summary>
    /// Konfigurerar kommersiella och concurrency-relaterade regler.
    /// </summary>
    public UpdateBuyerProposalRevisionRequestValidator()
    {
        RuleFor(x => x.CurrencyId).GreaterThan(0);
        RuleFor(x => x.CommercialTerms).MaximumLength(8000);
        RuleFor(x => x.BuyerFacingNotes).MaximumLength(8000);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);
        RuleFor(x => x.DisclosurePackageRevisionId).NotEqual(Guid.Empty).When(x => x.DisclosurePackageRevisionId.HasValue);
        RuleFor(x => x.RowVersion).NotEqual(0u);

        RuleFor(x => x)
            .Must(x =>
                !x.ValidFromUtc.HasValue
                || !x.ValidToUtc.HasValue
                || x.ValidToUtc.Value >= x.ValidFromUtc.Value)
            .WithMessage("ValidToUtc cannot be earlier than ValidFromUtc.");
    }
}