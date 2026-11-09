using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar filter, sortering och paginering för supply documents.
    /// </summary>
    public sealed class SupplyDocumentQueryRequestValidator : AbstractValidator<SupplyDocumentQueryRequest>
    {
        /// <summary>
        /// Skapar regler för document-sökningar.
        /// </summary>
        public SupplyDocumentQueryRequestValidator()
        {
            RuleFor(x => x.DocumentTypeId)
                .GreaterThan(0)
                .When(x => x.DocumentTypeId.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue);


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
