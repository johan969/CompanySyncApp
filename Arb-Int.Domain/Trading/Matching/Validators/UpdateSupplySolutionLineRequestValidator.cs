using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar coverage-uppdateringar på solution lines.
/// </summary>
public sealed class UpdateSupplySolutionLineRequestValidator : AbstractValidator<UpdateSupplySolutionLineRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för solution-line-uppdateringar.
    /// </summary>
    public UpdateSupplySolutionLineRequestValidator()
    {
        RuleFor(x => x.CoveredQuantity).GreaterThanOrEqualTo(0m);
        RuleFor(x => x.Notes).MaximumLength(4096);
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}