using FluentValidation;

namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Validerar statusändring för ett supplier material offering.
/// </summary>
public sealed class ChangeSupplierMaterialOfferingStatusRequestValidator : AbstractValidator<ChangeSupplierMaterialOfferingStatusRequest>
{
    /// <summary>
    /// Skapar regler för offering-status.
    /// </summary>
    public ChangeSupplierMaterialOfferingStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
