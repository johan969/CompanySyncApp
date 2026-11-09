using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar supporting document-kommandot för en supply analysis.
    /// </summary>
    public sealed class SetSupplyAnalysisDocumentRequestValidator : AbstractValidator<SetSupplyAnalysisDocumentRequest>
    {
        /// <summary>
        /// Skapar validatorn; null är tillåtet för att rensa document-kopplingen.
        /// </summary>
        public SetSupplyAnalysisDocumentRequestValidator()
        {
        }
    }
}
