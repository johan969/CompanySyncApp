using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar filter, sortering och paginering för document types.
    /// </summary>
    public sealed class DocumentTypeQueryRequestValidator : AbstractValidator<DocumentTypeQueryRequest>
    {
        /// <summary>
        /// Skapar regler för document type-sökningar.
        /// </summary>
        public DocumentTypeQueryRequestValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(512);

            RuleFor(x => x.Category)
                .IsInEnum()
                .When(x => x.Category.HasValue);

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
