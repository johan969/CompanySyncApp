using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar skapande av matchningsregelverk.
/// </summary>
public sealed class CreateMatchingRuleSetRequestValidator : AbstractValidator<CreateMatchingRuleSetRequest>
{
    /// <summary>
    /// Konfigurerar strukturella regler för nya regelverk.
    /// </summary>
    public CreateMatchingRuleSetRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.InitialVersionDescription).MaximumLength(4096);
        RuleFor(x => x.ConfigurationJson).NotEmpty().Must(MatchingValidationRules.BeValidJson).WithMessage("ConfigurationJson must contain valid JSON.");
        RuleFor(x => x).Must(x => MatchingValidationRules.HaveValidEffectiveWindow(x.EffectiveFromUtc, x.EffectiveToUtc)).WithMessage("EffectiveToUtc cannot be earlier than EffectiveFromUtc.");
    }
}