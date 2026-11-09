using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som skapar BuyerDemand.
    /// </summary>
    public sealed class CreateBuyerDemandRequestValidator : AbstractValidator<CreateBuyerDemandRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya BuyerDemand.
        /// </summary>
        public CreateBuyerDemandRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Must(x => !string.IsNullOrWhiteSpace(x));
            RuleFor(x => x.Description).NotEmpty().Must(x => !string.IsNullOrWhiteSpace(x));
            RuleFor(x => x.KnowledgeLevel).IsInEnum();
            RuleFor(x => x.Status).IsInEnum();
            RuleFor(x => x.Confidence).InclusiveBetween(0m, 1m).When(x => x.Confidence.HasValue);
            RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
            RuleFor(x => x.InitialStateChangedByUserId).NotEqual(Guid.Empty).When(x => x.InitialStateChangedByUserId.HasValue);
            RuleFor(x => x.InternalNotes).MaximumLength(4096);
            RuleFor(x => x.InitialStateNotes).MaximumLength(4096);

            RuleFor(x => x)
                .Must(x => !x.ValidFromUtc.HasValue || !x.ValidToUtc.HasValue || x.ValidToUtc.Value >= x.ValidFromUtc.Value)
                .WithMessage("ValidToUtc cannot be earlier than ValidFromUtc.");
        }
    }
}