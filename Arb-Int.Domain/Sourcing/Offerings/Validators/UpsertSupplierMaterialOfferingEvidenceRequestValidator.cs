using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar en källobservation som ska skapas eller uppdateras för ett supplier material offering.
    /// </summary>
    public sealed class UpsertSupplierMaterialOfferingEvidenceRequestValidator : AbstractValidator<UpsertSupplierMaterialOfferingEvidenceRequest>
    {
        /// <summary>
        /// Skapar regler för source-identitet, observationsdatum och evidensmetadata.
        /// </summary>
        public UpsertSupplierMaterialOfferingEvidenceRequestValidator()
        {
            RuleFor(x => x.EvidenceType)
                .IsInEnum();

            RuleFor(x => x.SourceSystem)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.SourceRecordKey)
                .NotEmpty()
                .MaximumLength(512);

            RuleFor(x => x.SourceUrl)
                .MaximumLength(2048);

            RuleFor(x => x.LastObservedUtc)
                .GreaterThanOrEqualTo(x => x.FirstObservedUtc);

            RuleFor(x => x.Notes)
                .MaximumLength(4096);

            RuleFor(x => x.ContentHash)
                .MaximumLength(128);
        }
    }
}
