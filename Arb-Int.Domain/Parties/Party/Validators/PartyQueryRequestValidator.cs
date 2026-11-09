using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar filtrering och paginering för Party-sökningar.
    /// </summary>
    public sealed class PartyQueryRequestValidator : AbstractValidator<PartyQueryRequest>
    {
        /// <summary>
        /// Konfigurerar valideringsreglerna för Party-sökningar.
        /// </summary>
        public PartyQueryRequestValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(255);

            RuleFor(x => x.PartyType)
                .Must(value => value is null or PartyType.Organization or PartyType.Person)
                .WithMessage("PartyType must be Organization or Person.");

            RuleFor(x => x.SortBy)
                .IsInEnum();

            RuleFor(x => x.SortDirection)
                .IsInEnum();

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 200);
        }
    }
}