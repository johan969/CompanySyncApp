using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar metadatauppdateringar av matchningsregelverk.
/// </summary>
public sealed class UpdateMatchingRuleSetRequestValidator : AbstractValidator<UpdateMatchingRuleSetRequest>
{
    /// <summary>
    /// Konfigurerar metadatareglerna.
    /// </summary>
    public UpdateMatchingRuleSetRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}