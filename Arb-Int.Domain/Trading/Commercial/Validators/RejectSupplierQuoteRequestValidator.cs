using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar intern rejection av supplier quotes.
/// </summary>
public sealed class RejectSupplierQuoteRequestValidator : AbstractValidator<RejectSupplierQuoteRequest>
{
    /// <summary>
    /// Konfigurerar revision- och concurrency-reglerna.
    /// </summary>
    public RejectSupplierQuoteRequestValidator()
    {
        RuleFor(x => x.SupplierQuoteRevisionId).NotEmpty();
        RuleFor(x => x.SupplierQuoteRowVersion).NotEqual(0u);
        RuleFor(x => x.SupplierQuoteRevisionRowVersion).NotEqual(0u);
    }
}