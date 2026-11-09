using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar filter, sortering och paginering för supply certifications.
    /// </summary>
    public sealed class SupplyCertificationQueryRequestValidator : AbstractValidator<SupplyCertificationQueryRequest>
    {
        /// <summary>
        /// Skapar regler för certification-sökningar.
        /// </summary>
        public SupplyCertificationQueryRequestValidator()
        {
            RuleFor(x => x.CertificationSchemeId).GreaterThan(0).When(x => x.CertificationSchemeId.HasValue);
            RuleFor(x => x.Scope).IsInEnum().When(x => x.Scope.HasValue);
            RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
            RuleFor(x => x.SortBy).IsInEnum();
            RuleFor(x => x.SortDirection).IsInEnum();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        }
    }
}
