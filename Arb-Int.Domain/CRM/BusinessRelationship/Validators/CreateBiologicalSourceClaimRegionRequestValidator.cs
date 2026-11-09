using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som lägger till geografiska platser på claims.
    /// </summary>
    public sealed class CreateBiologicalSourceClaimRegionRequestValidator : AbstractValidator<CreateBiologicalSourceClaimRegionRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya geografiska claim-kopplingar.
        /// </summary>
        public CreateBiologicalSourceClaimRegionRequestValidator()
        {
            RuleFor(x => x.GeographicLocationId)
                .NotEmpty();

            RuleFor(x => x.Role)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}