using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar giltighetsfönster för ett supply document.
    /// </summary>
    public sealed class SetSupplyDocumentValidityRequestValidator : AbstractValidator<SetSupplyDocumentValidityRequest>
    {
        /// <summary>
        /// Skapar regler för giltighetens start och slut.
        /// </summary>
        public SetSupplyDocumentValidityRequestValidator()
        {
            RuleFor(x => x.ValidToUtc)
                .GreaterThanOrEqualTo(x => x.ValidFromUtc!.Value)
                .When(x => x.ValidFromUtc.HasValue && x.ValidToUtc.HasValue);
        }
    }
}
