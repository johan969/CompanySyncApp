using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar uppdateringar av Proposed supply-solution-steps.
/// </summary>
public sealed class UpdateSupplySolutionStepRequestValidator : AbstractValidator<UpdateSupplySolutionStepRequest>
{
    /// <summary>
    /// Konfigurerar strukturella och concurrency-relaterade regler.
    /// </summary>
    public UpdateSupplySolutionStepRequestValidator()
    {
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
        RuleFor(x => x.RowVersion).NotEqual(0u);

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
    private static bool HaveValidInputQuantity(UpdateSupplySolutionStepRequest request)
    {
        return (!request.ExpectedInputQuantity.HasValue && !request.InputQuantityUnitId.HasValue)
            || (request.ExpectedInputQuantity >= 0m && request.InputQuantityUnitId.HasValue);
    }

    /// <summary>
    /// Kontrollerar outputkvantitetens quantity/unit-par.
    /// </summary>
    private static bool HaveValidOutputQuantity(UpdateSupplySolutionStepRequest request)
    {
        return (!request.ExpectedOutputQuantity.HasValue && !request.OutputQuantityUnitId.HasValue)
            || (request.ExpectedOutputQuantity >= 0m && request.OutputQuantityUnitId.HasValue);
    }

    /// <summary>
    /// Kontrollerar det planerade tidsfönstret.
    /// </summary>
    private static bool HaveValidPlannedWindow(UpdateSupplySolutionStepRequest request)
    {
        return !request.PlannedStartUtc.HasValue
            || !request.PlannedEndUtc.HasValue
            || request.PlannedEndUtc.Value >= request.PlannedStartUtc.Value;
    }

    /// <summary>
    /// Kontrollerar provider-kopplingen för en process capability.
    /// </summary>
    private static bool HaveValidCapabilityProvider(UpdateSupplySolutionStepRequest request)
    {
        return !request.SupplierProcessCapabilityId.HasValue
            || request.ProviderBusinessRelationshipId.HasValue;
    }
}