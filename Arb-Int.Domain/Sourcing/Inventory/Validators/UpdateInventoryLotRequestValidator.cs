using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar redigerbar metadata för en inventory lot.
    /// </summary>
    public sealed class UpdateInventoryLotRequestValidator : AbstractValidator<UpdateInventoryLotRequest>
    {
        /// <summary>
        /// Skapar regler för lotens redigerbara metadata.
        /// </summary>
        public UpdateInventoryLotRequestValidator()
        {
            RuleFor(x => x.SupplierLotNumber).MaximumLength(255);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.WarehouseReference).MaximumLength(255);
            RuleFor(x => x.InternalNotes).MaximumLength(4096);
        }
    }
}
