using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar active-status för en document type.
    /// </summary>
    public sealed class SetDocumentTypeActiveRequestValidator : AbstractValidator<SetDocumentTypeActiveRequest>
    {
        /// <summary>
        /// Skapar validatorn för explicit active-status.
        /// </summary>
        public SetDocumentTypeActiveRequestValidator()
        {
        }
    }
}
