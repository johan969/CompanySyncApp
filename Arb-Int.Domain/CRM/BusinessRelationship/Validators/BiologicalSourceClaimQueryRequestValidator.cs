using FluentValidation;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Validerar filter, sortering och paginering för biologiska source-claims.
    /// </summary>
    public sealed class BiologicalSourceClaimQueryRequestValidator : AbstractValidator<BiologicalSourceClaimQueryRequest>
    {
        /// <summary>
        /// Skapar regler för claim-sökningar.
        /// </summary>
        public BiologicalSourceClaimQueryRequestValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(512);

            RuleFor(x => x.BiologicalSourceId)
                .NotEmpty()
                .When(x => x.BiologicalSourceId.HasValue);

            RuleFor(x => x.BusinessRelationshipId)
                .NotEmpty()
                .When(x => x.BusinessRelationshipId.HasValue);

            RuleFor(x => x.BusinessRelationshipTypeCode)
                .MaximumLength(64);

            RuleFor(x => x.RelationshipKind)
                .IsInEnum()
                .When(x => x.RelationshipKind.HasValue);

            RuleFor(x => x.MappingStatus)
                .IsInEnum()
                .When(x => x.MappingStatus.HasValue);

            RuleFor(x => x.VerificationStatus)
                .IsInEnum()
                .When(x => x.VerificationStatus.HasValue);

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
