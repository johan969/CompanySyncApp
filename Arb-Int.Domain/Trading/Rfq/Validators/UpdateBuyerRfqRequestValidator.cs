using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar metadataändringar på BuyerRfq.
/// </summary>
public sealed class UpdateBuyerRfqRequestValidator : AbstractValidator<UpdateBuyerRfqRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för BuyerRfq-uppdateringar.
    /// </summary>
    public UpdateBuyerRfqRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Must(x => !string.IsNullOrWhiteSpace(x));
        RuleFor(x => x.Description).NotNull();
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.Visibility).IsInEnum();
        RuleFor(x => x.BuyerReference).MaximumLength(255);
        RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}