using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar uppdateringar av Draft-revisioner.
/// </summary>
public sealed class UpdateSupplierQuoteRevisionRequestValidator : AbstractValidator<UpdateSupplierQuoteRevisionRequest>
{
    /// <summary>
    /// Konfigurerar revisionens kommersiella och concurrency-relaterade regler.
    /// </summary>
    public UpdateSupplierQuoteRevisionRequestValidator()
    {
        RuleFor(x => x.CurrencyId).GreaterThan(0);
        RuleFor(x => x.IncotermRuleId).GreaterThan(0).When(x => x.IncotermRuleId.HasValue);
        RuleFor(x => x.NamedPlace).MaximumLength(512);
        RuleFor(x => x.PaymentTerms).MaximumLength(2048);
        RuleFor(x => x.OriginalDocumentReference).MaximumLength(2048);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);
        RuleFor(x => x.RowVersion).NotEqual(0u);

        RuleFor(x => x)
            .Must(x => SupplierQuoteValidationRules.HaveValidWindow(x.ValidFromUtc, x.ValidToUtc))
            .WithMessage("ValidToUtc cannot be earlier than ValidFromUtc.");
    }
}