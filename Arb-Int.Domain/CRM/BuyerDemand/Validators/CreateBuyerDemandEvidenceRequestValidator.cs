using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som skapar BuyerDemand-evidens.
    /// </summary>
    public sealed class CreateBuyerDemandEvidenceRequestValidator : AbstractValidator<CreateBuyerDemandEvidenceRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya evidensposter.
        /// </summary>
        public CreateBuyerDemandEvidenceRequestValidator()
        {
            RuleFor(x => x.SourceKind).IsInEnum();
            RuleFor(x => x.Position).IsInEnum();
            RuleFor(x => x.ReferenceEntryId).NotEqual(Guid.Empty).When(x => x.ReferenceEntryId.HasValue);
            RuleFor(x => x.CrmActivityId).NotEqual(Guid.Empty).When(x => x.CrmActivityId.HasValue);
            RuleFor(x => x.SourceUri).MaximumLength(2048);
            RuleFor(x => x.SourceExternalId).MaximumLength(255);
            RuleFor(x => x.Excerpt).MaximumLength(4096);
            RuleFor(x => x.Notes).MaximumLength(4096);
            RuleFor(x => x.Confidence).InclusiveBetween(0m, 1m).When(x => x.Confidence.HasValue);
            RuleFor(x => x.EvidenceHash).MaximumLength(128);
        }
    }
}