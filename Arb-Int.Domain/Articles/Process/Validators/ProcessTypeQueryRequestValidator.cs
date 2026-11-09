using FluentValidation;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Validerar filter, sortering och paginering för process types.
    /// </summary>
    public sealed class ProcessTypeQueryRequestValidator : AbstractValidator<ProcessTypeQueryRequest>
    {
        /// <summary>
        /// Skapar regler för process type-sökningar.
        /// </summary>
        public ProcessTypeQueryRequestValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(512);

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
