using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar affärsmässiga state-förändringar på BuyerDemand.
    /// </summary>
    public sealed class ChangeBuyerDemandStateRequestValidator : AbstractValidator<ChangeBuyerDemandStateRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för state-förändringar.
        /// </summary>
        public ChangeBuyerDemandStateRequestValidator()
        {
            RuleFor(x => x.KnowledgeLevel).IsInEnum();
            RuleFor(x => x.Status).IsInEnum();
            RuleFor(x => x.BuyerDemandEvidenceId).NotEqual(Guid.Empty).When(x => x.BuyerDemandEvidenceId.HasValue);
            RuleFor(x => x.Confidence).InclusiveBetween(0m, 1m).When(x => x.Confidence.HasValue);
            RuleFor(x => x.ChangedByUserId).NotEqual(Guid.Empty).When(x => x.ChangedByUserId.HasValue);
            RuleFor(x => x.Notes).MaximumLength(4096);
            RuleFor(x => x.RowVersion).NotEqual(0u);
        }
    }
}