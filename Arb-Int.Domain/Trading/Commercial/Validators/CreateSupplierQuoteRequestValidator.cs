using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar skapande av supplier quotes och deras första revision.
/// </summary>
public sealed class CreateSupplierQuoteRequestValidator : AbstractValidator<CreateSupplierQuoteRequest>
{
    /// <summary>
    /// Konfigurerar strukturella regler för root och revision 1.
    /// </summary>
    public CreateSupplierQuoteRequestValidator()
    {
        RuleFor(x => x.BuyerRfqRevisionId).NotEqual(Guid.Empty).When(x => x.BuyerRfqRevisionId.HasValue);
        RuleFor(x => x.QuoteNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.SupplierReference).MaximumLength(255);
        RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);
        RuleFor(x => x.CurrencyId).GreaterThan(0);
        RuleFor(x => x.IncotermRuleId).GreaterThan(0).When(x => x.IncotermRuleId.HasValue);
        RuleFor(x => x.NamedPlace).MaximumLength(512);
        RuleFor(x => x.PaymentTerms).MaximumLength(2048);
        RuleFor(x => x.OriginalDocumentReference).MaximumLength(2048);
        RuleFor(x => x.RevisionInternalNotes).MaximumLength(4096);

        RuleFor(x => x)
            .Must(x => SupplierQuoteValidationRules.HaveValidWindow(x.ValidFromUtc, x.ValidToUtc))
            .WithMessage("ValidToUtc cannot be earlier than ValidFromUtc.");
    }
}