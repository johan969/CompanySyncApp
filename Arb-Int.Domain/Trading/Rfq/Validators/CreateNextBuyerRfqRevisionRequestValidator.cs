using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som skapar nästa RFQ-revision.
/// </summary>
public sealed class CreateNextBuyerRfqRevisionRequestValidator : AbstractValidator<CreateNextBuyerRfqRevisionRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för nästa revision.
    /// </summary>
    public CreateNextBuyerRfqRevisionRequestValidator()
    {
        RuleFor(x => x.BuyerRfqRowVersion).NotEqual(0u);
    }
}