using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar supporting document-kommandot för en supply certification.
    /// </summary>
    public sealed class SetSupplyCertificationDocumentRequestValidator : AbstractValidator<SetSupplyCertificationDocumentRequest>
    {
        /// <summary>
        /// Skapar validatorn; null är tillåtet för att rensa document-kopplingen.
        /// </summary>
        public SetSupplyCertificationDocumentRequestValidator()
        {
        }
    }
}
