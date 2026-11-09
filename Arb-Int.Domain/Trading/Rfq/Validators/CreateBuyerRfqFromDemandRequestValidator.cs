using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som konverterar BuyerDemand till BuyerRfq.
/// </summary>
public sealed class CreateBuyerRfqFromDemandRequestValidator : AbstractValidator<CreateBuyerRfqFromDemandRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för BuyerDemand-konvertering.
    /// </summary>
    public CreateBuyerRfqFromDemandRequestValidator()
    {
        RuleFor(x => x.RfqNumber).NotEmpty().Must(x => !string.IsNullOrWhiteSpace(x)).MaximumLength(100);
        RuleFor(x => x.Name).Must(x => x == null || !string.IsNullOrWhiteSpace(x));
        RuleFor(x => x.Description).Must(x => x == null || !string.IsNullOrWhiteSpace(x));
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.Visibility).IsInEnum();
        RuleFor(x => x.BuyerReference).MaximumLength(255);
        RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);
        RuleFor(x => x.RevisionSummary).MaximumLength(4096);
        RuleFor(x => x.BuyerOriginalText).MaximumLength(16000);
        RuleFor(x => x.InternalInterpretation).MaximumLength(16000);
        RuleFor(x => x.ConvertedByUserId).NotEqual(Guid.Empty).When(x => x.ConvertedByUserId.HasValue);
        RuleFor(x => x.ConversionNotes).MaximumLength(4096);
        RuleFor(x => x.BuyerDemandRowVersion).NotEqual(0u);
    }
}