using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som uppdaterar geografiska claim-kopplingar.
    /// </summary>
    public sealed class UpdateBiologicalSourceClaimRegionRequestValidator : AbstractValidator<UpdateBiologicalSourceClaimRegionRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för geografiska claim-uppdateringar.
        /// </summary>
        public UpdateBiologicalSourceClaimRegionRequestValidator()
        {
            RuleFor(x => x.Role)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}