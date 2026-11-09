using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar metadataändringar för step-dependencies.
/// </summary>
public sealed class UpdateSupplySolutionStepDependencyRequestValidator : AbstractValidator<UpdateSupplySolutionStepDependencyRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för dependency-uppdateringar.
    /// </summary>
    public UpdateSupplySolutionStepDependencyRequestValidator()
    {
        RuleFor(x => x.Notes).MaximumLength(4096);
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}