using FluentValidation;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Validerar filter, sortering och paginering för materials.
    /// </summary>
    public sealed class MaterialQueryRequestValidator : AbstractValidator<MaterialQueryRequest>
    {
        /// <summary>
        /// Skapar regler för material-sökningar.
        /// </summary>
        public MaterialQueryRequestValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(512);

            RuleFor(x => x.MaterialTypeId)
                .GreaterThan(0)
                .When(x => x.MaterialTypeId.HasValue);

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
