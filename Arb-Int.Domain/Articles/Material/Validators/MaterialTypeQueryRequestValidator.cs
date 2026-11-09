using FluentValidation;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Validerar filter, sortering och paginering för material types.
    /// </summary>
    public sealed class MaterialTypeQueryRequestValidator : AbstractValidator<MaterialTypeQueryRequest>
    {
        /// <summary>
        /// Skapar regler för material type-sökningar.
        /// </summary>
        public MaterialTypeQueryRequestValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(512);

            RuleFor(x => x.ParentMaterialTypeId)
                .GreaterThan(0)
                .When(x => x.ParentMaterialTypeId.HasValue);

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
