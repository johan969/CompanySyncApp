using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar sökningar efter BuyerDemand.
    /// </summary>
    public sealed class BuyerDemandQueryRequestValidator : AbstractValidator<BuyerDemandQueryRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för BuyerDemand-filtrering och paginering.
        /// </summary>
        public BuyerDemandQueryRequestValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(255);

            RuleFor(x => x.BusinessRelationshipId)
                .NotEqual(Guid.Empty)
                .When(x => x.BusinessRelationshipId.HasValue);

            RuleFor(x => x.CounterpartyId)
                .NotEqual(Guid.Empty)
                .When(x => x.CounterpartyId.HasValue);

            RuleFor(x => x.KnowledgeLevel)
                .IsInEnum()
                .When(x => x.KnowledgeLevel.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue);

            RuleFor(x => x.OwnerUserId)
                .NotEqual(Guid.Empty)
                .When(x => x.OwnerUserId.HasValue);

            RuleFor(x => x.MinConfidence)
                .InclusiveBetween(0m, 1m)
                .When(x => x.MinConfidence.HasValue);

            RuleFor(x => x.MaxConfidence)
                .InclusiveBetween(0m, 1m)
                .When(x => x.MaxConfidence.HasValue);

            RuleFor(x => x)
                .Must(x => !x.MinConfidence.HasValue
                    || !x.MaxConfidence.HasValue
                    || x.MinConfidence.Value <= x.MaxConfidence.Value)
                .WithMessage("MinConfidence must be less than or equal to MaxConfidence.");

            RuleFor(x => x.SortBy)
                .IsInEnum();

            RuleFor(x => x.SortDirection)
                .IsInEnum();

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 200);
        }
    }
}
