using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar metadataändringar på en RFQ-revision.
/// </summary>
public sealed class UpdateBuyerRfqRevisionRequestValidator : AbstractValidator<UpdateBuyerRfqRevisionRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för revisionsuppdateringar.
    /// </summary>
    public UpdateBuyerRfqRevisionRequestValidator()
    {
        RuleFor(x => x.Summary).MaximumLength(4096);
        RuleFor(x => x.BuyerOriginalText).MaximumLength(16000);
        RuleFor(x => x.InternalInterpretation).MaximumLength(16000);
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}