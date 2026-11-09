using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar request för att påbörja intern supplier-quote-review.
/// </summary>
public sealed class BeginSupplierQuoteReviewRequestValidator : AbstractValidator<BeginSupplierQuoteReviewRequest>
{
    /// <summary>
    /// Konfigurerar revision- och concurrency-reglerna.
    /// </summary>
    public BeginSupplierQuoteReviewRequestValidator()
    {
        RuleFor(x => x.SupplierQuoteRevisionId).NotEmpty();
        RuleFor(x => x.SupplierQuoteRowVersion).NotEqual(0u);
        RuleFor(x => x.SupplierQuoteRevisionRowVersion).NotEqual(0u);
    }
}