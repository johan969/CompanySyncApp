using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar requests som skapar RFQ-leveranstillfällen.
/// </summary>
public sealed class CreateBuyerRfqDeliveryRequestValidator : AbstractValidator<CreateBuyerRfqDeliveryRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för nya leveranstillfällen.
    /// </summary>
    public CreateBuyerRfqDeliveryRequestValidator()
    {
        RuleFor(x => x.DestinationAddressId).NotEqual(Guid.Empty).When(x => x.DestinationAddressId.HasValue);
        RuleFor(x => x.DestinationLocationId).NotEqual(Guid.Empty).When(x => x.DestinationLocationId.HasValue);
        RuleFor(x => x.RequestedDeliveryFromUtc).NotEqual(default(DateTimeOffset));
        RuleFor(x => x.RequestedDeliveryToUtc).NotEqual(default(DateTimeOffset));
        RuleFor(x => x.IncotermRuleId).GreaterThan(0).When(x => x.IncotermRuleId.HasValue);
        RuleFor(x => x.NamedPlace).MaximumLength(512);
        RuleFor(x => x.PackagingNotes).MaximumLength(4096);
        RuleFor(x => x.DeliveryNotes).MaximumLength(4096);

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
    private static bool HaveValidDeliveryWindow(CreateBuyerRfqDeliveryRequest request)
    {
        return request.RequestedDeliveryToUtc >= request.RequestedDeliveryFromUtc;
    }

    /// <summary>
    /// Kontrollerar temperaturspannet när både minimum och maximum anges.
    /// </summary>
    private static bool HaveValidTemperatureRange(CreateBuyerRfqDeliveryRequest request)
    {
        return !request.MinimumTemperatureCelsius.HasValue
            || !request.MaximumTemperatureCelsius.HasValue
            || request.MaximumTemperatureCelsius.Value >= request.MinimumTemperatureCelsius.Value;
    }
}