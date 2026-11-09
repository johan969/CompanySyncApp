using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar sökningar efter affärsrelationer.
    /// </summary>
    public sealed class BusinessRelationshipQueryRequestValidator : AbstractValidator<BusinessRelationshipQueryRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för filtrering och paginering.
        /// </summary>
        public BusinessRelationshipQueryRequestValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(255);

            RuleFor(x => x.BusinessRelationshipTypeId)
                .GreaterThan(0)
                .When(x => x.BusinessRelationshipTypeId.HasValue);

            RuleFor(x => x.BusinessRelationshipStageId)
                .GreaterThan(0)
                .When(x => x.BusinessRelationshipStageId.HasValue);

            RuleFor(x => x.Priority)
                .IsInEnum()
                .When(x => x.Priority.HasValue);

            RuleFor(x => x.OwnerUserId)
                .NotEqual(Guid.Empty)
                .When(x => x.OwnerUserId.HasValue);

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