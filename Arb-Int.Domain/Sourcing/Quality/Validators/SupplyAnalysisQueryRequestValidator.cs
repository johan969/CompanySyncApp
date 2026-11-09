using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar filter, sortering och paginering för supply analyses.
    /// </summary>
    public sealed class SupplyAnalysisQueryRequestValidator : AbstractValidator<SupplyAnalysisQueryRequest>
    {
        /// <summary>
        /// Skapar regler för analysis-sökningar.
        /// </summary>
        public SupplyAnalysisQueryRequestValidator()
        {
            RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
            RuleFor(x => x.SortBy).IsInEnum();
            RuleFor(x => x.SortDirection).IsInEnum();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        }
    }
}
