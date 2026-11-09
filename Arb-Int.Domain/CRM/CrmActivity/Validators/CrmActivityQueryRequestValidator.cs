using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar sökningar efter CRM-aktiviteter.
    /// </summary>
    public sealed class CrmActivityQueryRequestValidator : AbstractValidator<CrmActivityQueryRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för aktivitetsfiltrering och paginering.
        /// </summary>
        public CrmActivityQueryRequestValidator()
        {
            RuleFor(x => x.RegardingPartyId).NotEqual(Guid.Empty).When(x => x.RegardingPartyId.HasValue);
            RuleFor(x => x.BusinessRelationshipId).NotEqual(Guid.Empty).When(x => x.BusinessRelationshipId.HasValue);
            RuleFor(x => x.ActivityType).IsInEnum().When(x => x.ActivityType.HasValue);
            RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
            RuleFor(x => x.Direction).IsInEnum().When(x => x.Direction.HasValue);
            RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
            RuleFor(x => x.Search).MaximumLength(512);
            RuleFor(x => x.SortBy).IsInEnum();
            RuleFor(x => x.SortDirection).IsInEnum();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);

            RuleFor(x => x)
                .Must(HaveValidOccurredInterval)
                .WithMessage("OccurredToUtc cannot be earlier than OccurredFromUtc.");
        }

        /// <summary>
        /// Kontrollerar att aktivitetens sökintervall är kronologiskt giltigt.
        /// </summary>
        private static bool HaveValidOccurredInterval(CrmActivityQueryRequest request)
        {
            return !request.OccurredFromUtc.HasValue
                || !request.OccurredToUtc.HasValue
                || request.OccurredToUtc.Value >= request.OccurredFromUtc.Value;
        }
    }
}