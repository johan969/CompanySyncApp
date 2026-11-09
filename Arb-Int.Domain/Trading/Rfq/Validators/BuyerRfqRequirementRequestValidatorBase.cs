using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Innehåller gemensamma FluentValidation-regler för alla RFQ-krav.
/// </summary>
public abstract class BuyerRfqRequirementRequestValidatorBase<TRequest> : AbstractValidator<TRequest>
    where TRequest : BuyerRfqRequirementRequestBase
{
    /// <summary>
    /// Konfigurerar regler som gäller samtliga requirement-typer.
    /// </summary>
    public BuyerRfqRequirementRequestValidatorBase()
    {
        RuleFor(x => x.BuyerRfqLineId).NotEqual(Guid.Empty).When(x => x.BuyerRfqLineId.HasValue);
        RuleFor(x => x.Name).NotEmpty().Must(x => !string.IsNullOrWhiteSpace(x));
        RuleFor(x => x.Description).NotNull();
        RuleFor(x => x.Level).IsInEnum();
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0m);
        RuleFor(x => x.AlternativeGroupKey).MaximumLength(100);
        RuleFor(x => x.BuyerOriginalText).MaximumLength(4096);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);
    }
}

/// <summary>
/// Innehåller gemensamma uppdateringsregler för RowVersion-skyddade RFQ-krav.
/// </summary>
public abstract class UpdateBuyerRfqRequirementRequestValidatorBase<TRequest> : BuyerRfqRequirementRequestValidatorBase<TRequest>
    where TRequest : UpdateBuyerRfqRequirementRequestBase
{
    /// <summary>
    /// Lägger till optimistic concurrency-validering för uppdateringar.
    /// </summary>
    public UpdateBuyerRfqRequirementRequestValidatorBase()
    {
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}