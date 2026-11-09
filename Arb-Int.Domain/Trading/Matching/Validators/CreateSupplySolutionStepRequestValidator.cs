using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar nya supply-solution-steps.
/// </summary>
public sealed class CreateSupplySolutionStepRequestValidator : AbstractValidator<CreateSupplySolutionStepRequest>
{
    /// <summary>
    /// Konfigurerar regler som motsvarar step-modellens strukturella invariants.
    /// </summary>
    public CreateSupplySolutionStepRequestValidator()
    {
        RuleFor(x => x.SupplySolutionLineId).NotEqual(Guid.Empty).When(x => x.SupplySolutionLineId.HasValue);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.ProviderBusinessRelationshipId).NotEqual(Guid.Empty).When(x => x.ProviderBusinessRelationshipId.HasValue);
        RuleFor(x => x.SupplierProcessCapabilityId).NotEqual(Guid.Empty).When(x => x.SupplierProcessCapabilityId.HasValue);
        RuleFor(x => x.ProcessTypeId).GreaterThan(0).When(x => x.ProcessTypeId.HasValue);
        RuleFor(x => x.InputMaterialId).NotEqual(Guid.Empty).When(x => x.InputMaterialId.HasValue);
        RuleFor(x => x.OutputMaterialId).NotEqual(Guid.Empty).When(x => x.OutputMaterialId.HasValue);
        RuleFor(x => x.InputQuantityUnitId).GreaterThan(0).When(x => x.InputQuantityUnitId.HasValue);
        RuleFor(x => x.OutputQuantityUnitId).GreaterThan(0).When(x => x.OutputQuantityUnitId.HasValue);
        RuleFor(x => x.ExpectedYieldPercent).InclusiveBetween(0m, 100m).When(x => x.ExpectedYieldPercent.HasValue);
        RuleFor(x => x.FromLocationId).NotEqual(Guid.Empty).When(x => x.FromLocationId.HasValue);
        RuleFor(x => x.ToLocationId).NotEqual(Guid.Empty).When(x => x.ToLocationId.HasValue);

        RuleFor(x => x)
            .Must(HaveValidInputQuantity)
            .WithMessage("ExpectedInputQuantity and InputQuantityUnitId must be supplied together and quantity cannot be negative.");

        RuleFor(x => x)
            .Must(HaveValidOutputQuantity)
            .WithMessage("ExpectedOutputQuantity and OutputQuantityUnitId must be supplied together and quantity cannot be negative.");

        RuleFor(x => x)
            .Must(HaveValidPlannedWindow)
            .WithMessage("PlannedEndUtc cannot be earlier than PlannedStartUtc.");

        RuleFor(x => x)
            .Must(HaveValidCapabilityProvider)
            .WithMessage("ProviderBusinessRelationshipId is required when SupplierProcessCapabilityId is supplied.");

        RuleFor(x => x.ParametersJson)
            .Must(MatchingValidationRules.BeValidJson)
            .When(x => !string.IsNullOrWhiteSpace(x.ParametersJson))
            .WithMessage("ParametersJson must contain valid JSON.");
    }

    /// <summary>
    /// Kontrollerar inputkvantitetens quantity/unit-par.
    /// </summary>
    private static bool HaveValidInputQuantity(CreateSupplySolutionStepRequest request)
    {
        return (!request.ExpectedInputQuantity.HasValue && !request.InputQuantityUnitId.HasValue)
            || (request.ExpectedInputQuantity >= 0m && request.InputQuantityUnitId.HasValue);
    }

    /// <summary>
    /// Kontrollerar outputkvantitetens quantity/unit-par.
    /// </summary>
    private static bool HaveValidOutputQuantity(CreateSupplySolutionStepRequest request)
    {
        return (!request.ExpectedOutputQuantity.HasValue && !request.OutputQuantityUnitId.HasValue)
            || (request.ExpectedOutputQuantity >= 0m && request.OutputQuantityUnitId.HasValue);
    }

    /// <summary>
    /// Kontrollerar ordningen på det planerade tidsfönstret.
    /// </summary>
    private static bool HaveValidPlannedWindow(CreateSupplySolutionStepRequest request)
    {
        return !request.PlannedStartUtc.HasValue
            || !request.PlannedEndUtc.HasValue
            || request.PlannedEndUtc.Value >= request.PlannedStartUtc.Value;
    }

    /// <summary>
    /// Kontrollerar att en process capability alltid har en provider.
    /// </summary>
    private static bool HaveValidCapabilityProvider(CreateSupplySolutionStepRequest request)
    {
        return !request.SupplierProcessCapabilityId.HasValue
            || request.ProviderBusinessRelationshipId.HasValue;
    }
}