using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar grunddata för en ny inventory lot.
    /// </summary>
    public sealed class CreateInventoryLotRequestValidator : AbstractValidator<CreateInventoryLotRequest>
    {
        /// <summary>
        /// Skapar regler för lot-identitet, kvantiteter och metadata.
        /// </summary>
        public CreateInventoryLotRequestValidator()
        {
            RuleFor(x => x.LotNumber).NotEmpty().MaximumLength(255);
            RuleFor(x => x.SupplierLotNumber).MaximumLength(255);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.TotalQuantity).GreaterThanOrEqualTo(0m);
            RuleFor(x => x.AvailableQuantity).GreaterThanOrEqualTo(0m);
            RuleFor(x => x.ReservedQuantity).GreaterThanOrEqualTo(0m);
            RuleFor(x => x.QuantityUnitId).GreaterThan(0);
            RuleFor(x => x.WarehouseReference).MaximumLength(255);
            RuleFor(x => x.InternalNotes).MaximumLength(4096);
            RuleFor(x => x).Must(x => x.AvailableQuantity + x.ReservedQuantity <= x.TotalQuantity)
                .WithMessage("AvailableQuantity plus ReservedQuantity cannot exceed TotalQuantity.");
        }
    }
}
