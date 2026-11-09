using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar skapande av nästa supplier-quote-revision.
/// </summary>
public sealed class CreateNextSupplierQuoteRevisionRequestValidator : AbstractValidator<CreateNextSupplierQuoteRevisionRequest>
{
    /// <summary>
    /// Konfigurerar concurrency-regeln för quote-roten.
    /// </summary>
    public CreateNextSupplierQuoteRevisionRequestValidator()
    {
        RuleFor(x => x.SupplierQuoteRowVersion).NotEqual(0u);
    }
}