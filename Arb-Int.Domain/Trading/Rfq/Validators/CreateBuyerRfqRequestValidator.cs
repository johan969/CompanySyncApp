using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som skapar BuyerRfq.
/// </summary>
public sealed class CreateBuyerRfqRequestValidator : AbstractValidator<CreateBuyerRfqRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för nya BuyerRfq.
    /// </summary>
    public CreateBuyerRfqRequestValidator()
    {
        RuleFor(x => x.RfqNumber).NotEmpty().Must(x => !string.IsNullOrWhiteSpace(x)).MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().Must(x => !string.IsNullOrWhiteSpace(x));
        RuleFor(x => x.Description).NotNull();
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.Visibility).IsInEnum();
        RuleFor(x => x.BuyerReference).MaximumLength(255);
        RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);
        RuleFor(x => x.RevisionSummary).MaximumLength(4096);
        RuleFor(x => x.BuyerOriginalText).MaximumLength(16000);
        RuleFor(x => x.InternalInterpretation).MaximumLength(16000);
    }
}