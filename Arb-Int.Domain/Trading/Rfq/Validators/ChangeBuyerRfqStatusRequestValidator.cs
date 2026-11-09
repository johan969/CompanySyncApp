using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar statusändringar på BuyerRfq.
/// </summary>
public sealed class ChangeBuyerRfqStatusRequestValidator : AbstractValidator<ChangeBuyerRfqStatusRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för statusändringar.
    /// </summary>
    public ChangeBuyerRfqStatusRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}