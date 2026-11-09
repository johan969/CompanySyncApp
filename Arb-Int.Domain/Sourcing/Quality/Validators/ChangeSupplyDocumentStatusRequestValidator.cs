using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar statusändring för ett supply document.
    /// </summary>
    public sealed class ChangeSupplyDocumentStatusRequestValidator : AbstractValidator<ChangeSupplyDocumentStatusRequest>
    {
        /// <summary>
        /// Skapar regler för document-status.
        /// </summary>
        public ChangeSupplyDocumentStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum();
        }
    }
}
