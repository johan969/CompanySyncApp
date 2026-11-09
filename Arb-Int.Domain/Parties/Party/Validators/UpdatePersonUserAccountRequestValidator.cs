using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar metadataändringar på en användarkontokoppling.
    /// </summary>
    public sealed class UpdatePersonUserAccountRequestValidator : AbstractValidator<UpdatePersonUserAccountRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för metadataändringar.
        /// </summary>
        public UpdatePersonUserAccountRequestValidator()
        {
            RuleFor(x => x.Notes)
                .MaximumLength(4096);

            RuleFor(x => x.RowVersion)
                .NotEqual(0u);
        }
    }
}