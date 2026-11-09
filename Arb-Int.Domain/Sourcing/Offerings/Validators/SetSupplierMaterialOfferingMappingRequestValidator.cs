using FluentValidation;

namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Validerar materialmapping för ett supplier material offering.
/// </summary>
public sealed class SetSupplierMaterialOfferingMappingRequestValidator : AbstractValidator<SetSupplierMaterialOfferingMappingRequest>
{
    /// <summary>
    /// Skapar regler för offering-mapping.
    /// </summary>
    public SetSupplierMaterialOfferingMappingRequestValidator()
    {
        RuleFor(x => x.MappingStatus)
            .IsInEnum();

        RuleFor(x => x.MappingConfidence)
            .InclusiveBetween(0m, 1m)
            .When(x => x.MappingConfidence.HasValue);

        RuleFor(x => x.MappingNotes)
            .MaximumLength(4096);

        RuleFor(x => x.MappedMaterialId)
            .NotNull()
            .When(x => x.MappingStatus == SourcingMappingStatus.Confirmed)
            .WithMessage("MappedMaterialId is required when MappingStatus is Confirmed.");
    }
}
