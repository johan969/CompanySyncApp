using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som uppdaterar RFQ-leveranstillfällen.
/// </summary>
public sealed class UpdateBuyerRfqDeliveryRequestValidator : AbstractValidator<UpdateBuyerRfqDeliveryRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för uppdaterade leveranstillfällen.
    /// </summary>
    public UpdateBuyerRfqDeliveryRequestValidator()
    {
        RuleFor(x => x.DestinationAddressId).NotEqual(Guid.Empty).When(x => x.DestinationAddressId.HasValue);
        RuleFor(x => x.DestinationLocationId).NotEqual(Guid.Empty).When(x => x.DestinationLocationId.HasValue);
        RuleFor(x => x.RequestedDeliveryFromUtc).NotEqual(default(DateTimeOffset));
        RuleFor(x => x.RequestedDeliveryToUtc).NotEqual(default(DateTimeOffset));
        RuleFor(x => x.IncotermRuleId).GreaterThan(0).When(x => x.IncotermRuleId.HasValue);
        RuleFor(x => x.NamedPlace).MaximumLength(512);
        RuleFor(x => x.PackagingNotes).MaximumLength(4096);
        RuleFor(x => x.DeliveryNotes).MaximumLength(4096);
        RuleFor(x => x.RowVersion).NotEqual(0u);

        RuleFor(x => x)
            .Must(HaveValidDeliveryWindow)
            .WithMessage("RequestedDeliveryToUtc cannot be earlier than RequestedDeliveryFromUtc.");

        RuleFor(x => x)
            .Must(HaveValidTemperatureRange)
            .WithMessage("MaximumTemperatureCelsius cannot be lower than MinimumTemperatureCelsius.");
    }

    /// <summary>
    /// Kontrollerar att leveransfönstrets sluttid inte ligger före starttiden.
    /// </summary>
    private static bool HaveValidDeliveryWindow(UpdateBuyerRfqDeliveryRequest request)
    {
        return request.RequestedDeliveryToUtc >= request.RequestedDeliveryFromUtc;
    }

    /// <summary>
    /// Kontrollerar temperaturspannet när både minimum och maximum anges.
    /// </summary>
    private static bool HaveValidTemperatureRange(UpdateBuyerRfqDeliveryRequest request)
    {
        return !request.MinimumTemperatureCelsius.HasValue
            || !request.MaximumTemperatureCelsius.HasValue
            || request.MaximumTemperatureCelsius.Value >= request.MinimumTemperatureCelsius.Value;
    }
}