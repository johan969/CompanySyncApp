using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som lägger till claim-evidens.
    /// </summary>
    public sealed class CreateBiologicalSourceClaimEvidenceRequestValidator : AbstractValidator<CreateBiologicalSourceClaimEvidenceRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya evidensposter.
        /// </summary>
        public CreateBiologicalSourceClaimEvidenceRequestValidator()
        {
            RuleFor(x => x.EvidenceType)
                .IsInEnum();

            RuleFor(x => x.SourceSystem)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(100);

            RuleFor(x => x.SourceRecordKey)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(512);

            RuleFor(x => x.SourceUrl)
                .MaximumLength(2048);

            RuleFor(x => x.Notes)
                .MaximumLength(4096);

            RuleFor(x => x.ContentHash)
                .MaximumLength(128);
        }
    }
}