using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar filter, sortering och paginering för certification schemes.
    /// </summary>
    public sealed class CertificationSchemeQueryRequestValidator : AbstractValidator<CertificationSchemeQueryRequest>
    {
        /// <summary>
        /// Skapar regler för scheme-sökningar.
        /// </summary>
        public CertificationSchemeQueryRequestValidator()
        {
            RuleFor(x => x.Kind)
                .IsInEnum()
                .When(x => x.Kind.HasValue);

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
