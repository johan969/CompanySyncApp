using FluentValidation;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Validerar filter, sortering och paginering för biologiska källor.
    /// </summary>
    public sealed class BiologicalSourceQueryRequestValidator : AbstractValidator<BiologicalSourceQueryRequest>
    {
        /// <summary>
        /// Skapar regler för sökningar av biologiska källor.
        /// </summary>
        public BiologicalSourceQueryRequestValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(512);

            RuleFor(x => x.RecordKind)
                .IsInEnum()
                .When(x => x.RecordKind.HasValue);

            RuleFor(x => x.DeclaredRank)
                .IsInEnum()
                .When(x => x.DeclaredRank.HasValue);

            RuleFor(x => x.ParentBiologicalSourceId)
                .NotEmpty()
                .When(x => x.ParentBiologicalSourceId.HasValue);

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
