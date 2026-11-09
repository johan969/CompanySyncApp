using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar statusändring för en supply analysis.
    /// </summary>
    public sealed class ChangeSupplyAnalysisStatusRequestValidator : AbstractValidator<ChangeSupplyAnalysisStatusRequest>
    {
        /// <summary>
        /// Skapar regler för analysis status.
        /// </summary>
        public ChangeSupplyAnalysisStatusRequestValidator()
        {
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
