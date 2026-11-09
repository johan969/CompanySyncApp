using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar filter, sortering och paginering för inventory lots.
    /// </summary>
    public sealed class InventoryLotQueryRequestValidator : AbstractValidator<InventoryLotQueryRequest>
    {
        /// <summary>
        /// Skapar regler för lot-sökningar.
        /// </summary>
        public InventoryLotQueryRequestValidator()
        {
            RuleFor(x => x.Search).MaximumLength(512);
            RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
            RuleFor(x => x.SortBy).IsInEnum();
            RuleFor(x => x.SortDirection).IsInEnum();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        }
    }
}
