using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar statusändring för en inventory lot.
    /// </summary>
    public sealed class ChangeInventoryLotStatusRequestValidator : AbstractValidator<ChangeInventoryLotStatusRequest>
    {
        /// <summary>
        /// Skapar regler för lot-status.
        /// </summary>
        public ChangeInventoryLotStatusRequestValidator()
        {
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
