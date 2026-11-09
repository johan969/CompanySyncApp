using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som uppdaterar claim-evidens.
    /// </summary>
    public sealed class UpdateBiologicalSourceClaimEvidenceRequestValidator : AbstractValidator<UpdateBiologicalSourceClaimEvidenceRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för evidensuppdateringar.
        /// </summary>
        public UpdateBiologicalSourceClaimEvidenceRequestValidator()
        {
            RuleFor(x => x.EvidenceType)
                .IsInEnum();

            RuleFor(x => x.SourceUrl)
                .MaximumLength(2048);

            RuleFor(x => x.ObservedUtc)
                .NotEqual(default(DateTimeOffset));

            RuleFor(x => x.Notes)
                .MaximumLength(4096);

            RuleFor(x => x.ContentHash)
                .MaximumLength(128);
        }
    }
}