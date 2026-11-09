using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar target, qualifier och mätmetadata för ett nytt supply analysis result.
    /// </summary>
    public sealed class CreateSupplyAnalysisResultRequestValidator : AbstractValidator<CreateSupplyAnalysisResultRequest>
    {
        /// <summary>
        /// Skapar regler som speglar result-targetens databasconstraint utan att hitta på qualifier-semantik.
        /// </summary>
        public CreateSupplyAnalysisResultRequestValidator()
        {
            RuleFor(x => x)
                .Must(HasValidTarget)
                .WithMessage("Specify ConstituentId, CharacteristicId or AnalyteName, but never both ConstituentId and CharacteristicId.");

            RuleFor(x => x.Qualifier)
                .IsInEnum();

            RuleFor(x => x.Basis)
                .IsInEnum();

            RuleFor(x => x.VerificationState)
                .IsInEnum();

            RuleFor(x => x.DetectionLimit)
                .GreaterThanOrEqualTo(0m)
                .When(x => x.DetectionLimit.HasValue);

            RuleFor(x => x)
                .Must(x => !x.RangeMaximumValue.HasValue || !x.NumericValue.HasValue || x.RangeMaximumValue.Value >= x.NumericValue.Value)
                .WithMessage("RangeMaximumValue cannot be lower than NumericValue.");

            RuleFor(x => x.AnalyteName)
                .MaximumLength(512);

            RuleFor(x => x.TextValue)
                .MaximumLength(4096);

            RuleFor(x => x.AnalysisMethod)
                .MaximumLength(1000);

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }

        /// <summary>
        /// Tillåter raw analyte tillsammans med en canonical target men aldrig både constituent och characteristic.
        /// </summary>
        private static bool HasValidTarget(CreateSupplyAnalysisResultRequest request)
        {
            if (request.ConstituentId.HasValue && request.CharacteristicId.HasValue)
            {
                return false;
            }

            return request.ConstituentId.HasValue
                || request.CharacteristicId.HasValue
                || !string.IsNullOrWhiteSpace(request.AnalyteName);
        }
    }
}
