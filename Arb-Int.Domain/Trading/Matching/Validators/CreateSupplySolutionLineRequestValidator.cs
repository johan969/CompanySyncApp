using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar nya solution lines.
/// </summary>
public sealed class CreateSupplySolutionLineRequestValidator : AbstractValidator<CreateSupplySolutionLineRequest>
{
    /// <summary>
    /// Konfigurerar strukturella regler för solution lines.
    /// </summary>
    public CreateSupplySolutionLineRequestValidator()
    {
        RuleFor(x => x.MatchingRunLineId).NotEmpty();
        RuleFor(x => x.CoveredQuantity).GreaterThanOrEqualTo(0m);
        RuleFor(x => x.Notes).MaximumLength(4096);
    }
}