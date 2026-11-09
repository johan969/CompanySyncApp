using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar intern acceptans av supplier quotes.
/// </summary>
public sealed class AcceptSupplierQuoteInternallyRequestValidator : AbstractValidator<AcceptSupplierQuoteInternallyRequest>
{
    /// <summary>
    /// Konfigurerar revision- och concurrency-reglerna.
    /// </summary>
    public AcceptSupplierQuoteInternallyRequestValidator()
    {
        RuleFor(x => x.SupplierQuoteRevisionId).NotEmpty();
        RuleFor(x => x.SupplierQuoteRowVersion).NotEqual(0u);
        RuleFor(x => x.SupplierQuoteRevisionRowVersion).NotEqual(0u);
    }
}