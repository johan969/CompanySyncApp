using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar withdrawal av supplier quotes.
/// </summary>
public sealed class WithdrawSupplierQuoteRequestValidator : AbstractValidator<WithdrawSupplierQuoteRequest>
{
    /// <summary>
    /// Konfigurerar concurrency-regeln.
    /// </summary>
    public WithdrawSupplierQuoteRequestValidator()
    {
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}