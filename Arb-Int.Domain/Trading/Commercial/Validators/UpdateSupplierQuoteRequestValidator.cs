using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar metadataändringar på supplier-quote-roten.
/// </summary>
public sealed class UpdateSupplierQuoteRequestValidator : AbstractValidator<UpdateSupplierQuoteRequest>
{
    /// <summary>
    /// Konfigurerar metadata- och concurrency-regler.
    /// </summary>
    public UpdateSupplierQuoteRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.SupplierReference).MaximumLength(255);
        RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}