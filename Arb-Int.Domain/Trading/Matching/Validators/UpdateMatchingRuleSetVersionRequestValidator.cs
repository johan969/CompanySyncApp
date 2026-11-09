using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar uppdateringar av opublicerade regelversioner.
/// </summary>
public sealed class UpdateMatchingRuleSetVersionRequestValidator : AbstractValidator<UpdateMatchingRuleSetVersionRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för regelversionsuppdateringar.
    /// </summary>
    public UpdateMatchingRuleSetVersionRequestValidator()
    {
        RuleFor(x => x.Description).MaximumLength(4096);
        RuleFor(x => x.ConfigurationJson).NotEmpty().Must(MatchingValidationRules.BeValidJson).WithMessage("ConfigurationJson must contain valid JSON.");
        RuleFor(x => x.RowVersion).NotEqual(0u);
        RuleFor(x => x).Must(x => MatchingValidationRules.HaveValidEffectiveWindow(x.EffectiveFromUtc, x.EffectiveToUtc)).WithMessage("EffectiveToUtc cannot be earlier than EffectiveFromUtc.");
    }
}