using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som uppdaterar biologiska source-claims.
    /// </summary>
    public sealed class UpdateBiologicalSourceClaimRequestValidator : AbstractValidator<UpdateBiologicalSourceClaimRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för claim-uppdateringar.
        /// </summary>
        public UpdateBiologicalSourceClaimRequestValidator()
        {
            RuleFor(x => x.DeclaredName)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(512);

            RuleFor(x => x.RelationshipKind)
                .IsInEnum();

            RuleFor(x => x.FirstObservedUtc)
                .NotEqual(default(DateTimeOffset));

            RuleFor(x => x.LastObservedUtc)
                .NotEqual(default(DateTimeOffset))
                .GreaterThanOrEqualTo(x => x.FirstObservedUtc);

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}