using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som skapar affärsrelationer.
    /// </summary>
    public sealed class CreateBusinessRelationshipRequestValidator : AbstractValidator<CreateBusinessRelationshipRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya affärsrelationer.
        /// </summary>
        public CreateBusinessRelationshipRequestValidator()
        {
            RuleFor(x => x.CounterpartyId)
                .NotEmpty();

            RuleFor(x => x.BusinessRelationshipTypeId)
                .GreaterThan(0);

            RuleFor(x => x.BusinessRelationshipStageId)
                .GreaterThan(0);

            RuleFor(x => x.Priority)
                .IsInEnum()
                .When(x => x.Priority.HasValue);

            RuleFor(x => x.Score)
                .InclusiveBetween(0, 100)
                .When(x => x.Score.HasValue);

            RuleFor(x => x.LeadSource)
                .MaximumLength(255);

            RuleFor(x => x.OwnerUserId)
                .NotEqual(Guid.Empty)
                .When(x => x.OwnerUserId.HasValue);

            RuleFor(x => x.Notes)
                .MaximumLength(4096);

            RuleFor(x => x)
                .Must(HaveValidContactDates)
                .WithMessage("LastContactedUtc cannot be earlier than FirstContactedUtc.");
        }

        /// <summary>
        /// Kontrollerar att kontaktintervallets sluttid inte ligger före starttiden.
        /// </summary>
        private static bool HaveValidContactDates(CreateBusinessRelationshipRequest request)
        {
            return !request.FirstContactedUtc.HasValue
                || !request.LastContactedUtc.HasValue
                || request.LastContactedUtc.Value >= request.FirstContactedUtc.Value;
        }
    }
}