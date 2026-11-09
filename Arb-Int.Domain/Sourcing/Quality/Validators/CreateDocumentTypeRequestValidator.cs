using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar en ny document type.
    /// </summary>
    public sealed class CreateDocumentTypeRequestValidator : AbstractValidator<CreateDocumentTypeRequest>
    {
        /// <summary>
        /// Skapar regler för code, name, description och category.
        /// </summary>
        public CreateDocumentTypeRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .MaximumLength(128);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(512);

            RuleFor(x => x.Description)
                .NotNull();

            RuleFor(x => x.Category)
                .IsInEnum();
        }
    }
}
