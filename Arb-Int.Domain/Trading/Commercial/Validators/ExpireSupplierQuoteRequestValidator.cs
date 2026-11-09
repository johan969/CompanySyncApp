using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar explicit expiration av supplier quotes.
/// </summary>
public sealed class ExpireSupplierQuoteRequestValidator : AbstractValidator<ExpireSupplierQuoteRequest>
{
    /// <summary>
    /// Konfigurerar revision- och concurrency-reglerna.
    /// </summary>
    public ExpireSupplierQuoteRequestValidator()
    {
        RuleFor(x => x.SupplierQuoteRevisionId).NotEmpty();
        RuleFor(x => x.SupplierQuoteRowVersion).NotEqual(0u);
        RuleFor(x => x.SupplierQuoteRevisionRowVersion).NotEqual(0u);
    }
}