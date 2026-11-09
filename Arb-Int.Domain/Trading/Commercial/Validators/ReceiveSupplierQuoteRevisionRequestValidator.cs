using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar mottagning och frysning av en supplier-quote-revision.
/// </summary>
public sealed class ReceiveSupplierQuoteRevisionRequestValidator : AbstractValidator<ReceiveSupplierQuoteRevisionRequest>
{
    /// <summary>
    /// Konfigurerar concurrency-reglerna för quote och revision.
    /// </summary>
    public ReceiveSupplierQuoteRevisionRequestValidator()
    {
        RuleFor(x => x.SupplierQuoteRowVersion).NotEqual(0u);
        RuleFor(x => x.SupplierQuoteRevisionRowVersion).NotEqual(0u);
    }
}