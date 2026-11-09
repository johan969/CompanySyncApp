using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar filter, sortering och paginering för supplier process capabilities.
    /// </summary>
    public sealed class SupplierProcessCapabilityQueryRequestValidator : AbstractValidator<SupplierProcessCapabilityQueryRequest>
    {
        /// <summary>
        /// Skapar regler för capability-sökningar.
        /// </summary>
        public SupplierProcessCapabilityQueryRequestValidator()
        {
            RuleFor(x => x.Search).MaximumLength(512);
            RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
            RuleFor(x => x.MappingStatus).IsInEnum().When(x => x.MappingStatus.HasValue);
            RuleFor(x => x.VerificationStatus).IsInEnum().When(x => x.VerificationStatus.HasValue);
            RuleFor(x => x.BiologicalSourceId).NotEmpty().When(x => x.BiologicalSourceId.HasValue);
            RuleFor(x => x.BusinessRelationshipId).NotEmpty().When(x => x.BusinessRelationshipId.HasValue);
            RuleFor(x => x.SortBy).IsInEnum();
            RuleFor(x => x.SortDirection).IsInEnum();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        }
    }
}
