using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar redigerbar metadata för en document type.
    /// </summary>
    public sealed class UpdateDocumentTypeRequestValidator : AbstractValidator<UpdateDocumentTypeRequest>
    {
        /// <summary>
        /// Skapar regler för name, description och category.
        /// </summary>
        public UpdateDocumentTypeRequestValidator()
        {
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
