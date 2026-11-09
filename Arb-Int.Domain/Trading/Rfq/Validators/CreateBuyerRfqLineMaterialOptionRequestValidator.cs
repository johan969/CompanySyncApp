using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som skapar materialalternativ.
/// </summary>
public sealed class CreateBuyerRfqLineMaterialOptionRequestValidator : AbstractValidator<CreateBuyerRfqLineMaterialOptionRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för nya materialalternativ.
    /// </summary>
    public CreateBuyerRfqLineMaterialOptionRequestValidator()
    {
        RuleFor(x => x.OptionKind).IsInEnum();
        RuleFor(x => x.MaterialId).NotEqual(Guid.Empty).When(x => x.MaterialId.HasValue);
        RuleFor(x => x.MaterialTypeId).GreaterThan(0).When(x => x.MaterialTypeId.HasValue);
        RuleFor(x => x.BiologicalSourceId).NotEqual(Guid.Empty).When(x => x.BiologicalSourceId.HasValue);
        RuleFor(x => x.BiologicalSourceClaimId).NotEqual(Guid.Empty).When(x => x.BiologicalSourceClaimId.HasValue);
        RuleFor(x => x.BuyerText).MaximumLength(2048);
        RuleFor(x => x.AlternativeGroupKey).MaximumLength(100);

        RuleFor(x => x)
            .Must(HaveTarget)
            .WithMessage("At least one material option target must be specified.");
    }

    /// <summary>
    /// Kontrollerar att alternativet innehåller minst ett matchningsbart target.
    /// </summary>
    private static bool HaveTarget(CreateBuyerRfqLineMaterialOptionRequest request)
    {
        return request.MaterialId.HasValue
            || request.MaterialTypeId.HasValue
            || request.BiologicalSourceId.HasValue
            || request.BiologicalSourceClaimId.HasValue
            || !string.IsNullOrWhiteSpace(request.BuyerText);
    }
}