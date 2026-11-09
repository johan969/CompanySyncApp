using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar nya buyer-proposal-lines.
/// </summary>
public sealed class CreateBuyerProposalLineRequestValidator : AbstractValidator<CreateBuyerProposalLineRequest>
{
    /// <summary>
    /// Konfigurerar strukturella och monetära inputregler.
    /// </summary>
    public CreateBuyerProposalLineRequestValidator()
    {
        RuleFor(x => x.BuyerRfqLineId).NotEmpty();
        RuleFor(x => x.SupplySolutionLineId).NotEqual(Guid.Empty).When(x => x.SupplySolutionLineId.HasValue);
        RuleFor(x => x.MaterialId).NotEqual(Guid.Empty).When(x => x.MaterialId.HasValue);
        RuleFor(x => x.ProductId).NotEqual(Guid.Empty).When(x => x.ProductId.HasValue);
        RuleFor(x => x.LineNumber).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0m);
        RuleFor(x => x.QuantityUnitId).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0m);
        RuleFor(x => x.BuyerFacingNotes).MaximumLength(4096);

        RuleFor(x => x)
            .Must(x =>
                !x.EstimatedDeliveryFromUtc.HasValue
                || !x.EstimatedDeliveryToUtc.HasValue
                || x.EstimatedDeliveryToUtc.Value >= x.EstimatedDeliveryFromUtc.Value)
            .WithMessage("EstimatedDeliveryToUtc cannot be earlier than EstimatedDeliveryFromUtc.");
    }
}