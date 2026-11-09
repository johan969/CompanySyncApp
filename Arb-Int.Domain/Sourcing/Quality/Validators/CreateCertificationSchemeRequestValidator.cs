using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar en ny certification scheme-version.
    /// </summary>
    public sealed class CreateCertificationSchemeRequestValidator : AbstractValidator<CreateCertificationSchemeRequest>
    {
        /// <summary>
        /// Skapar regler för identitet, kind och validity.
        /// </summary>
        public CreateCertificationSchemeRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Description)
                .NotNull();

            RuleFor(x => x.Kind)
                .IsInEnum();

            RuleFor(x => x.StandardOwner)
                .NotEmpty();

            RuleFor(x => x.StandardVersion)
                .NotEmpty();

            RuleFor(x => x.ValidToUtc)
                .GreaterThanOrEqualTo(x => x.ValidFromUtc!.Value)
                .When(x => x.ValidFromUtc.HasValue && x.ValidToUtc.HasValue);
        }
    }
}
