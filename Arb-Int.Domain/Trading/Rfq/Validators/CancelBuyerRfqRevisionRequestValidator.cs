using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som avbryter draft-revisioner.
/// </summary>
public sealed class CancelBuyerRfqRevisionRequestValidator : AbstractValidator<CancelBuyerRfqRevisionRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för avbrutna revisioner.
    /// </summary>
    public CancelBuyerRfqRevisionRequestValidator()
    {
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}