using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar validity-fönster för en supply certification.
    /// </summary>
    public sealed class SetSupplyCertificationValidityRequestValidator : AbstractValidator<SetSupplyCertificationValidityRequest>
    {
        /// <summary>
        /// Skapar regler för validity start och end.
        /// </summary>
        public SetSupplyCertificationValidityRequestValidator()
        {
            RuleFor(x => x.ValidToUtc)
                .GreaterThanOrEqualTo(x => x.ValidFromUtc!.Value)
                .When(x => x.ValidFromUtc.HasValue && x.ValidToUtc.HasValue);
        }
    }
}
