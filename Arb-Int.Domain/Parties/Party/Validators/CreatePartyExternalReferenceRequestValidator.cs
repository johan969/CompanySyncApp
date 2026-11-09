using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som skapar eller upsertar externa Party-referenser.
    /// </summary>
    public sealed class CreatePartyExternalReferenceRequestValidator : AbstractValidator<CreatePartyExternalReferenceRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för externa Party-referenser.
        /// </summary>
        public CreatePartyExternalReferenceRequestValidator()
        {
            RuleFor(x => x.SourceSystem)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(100);

            RuleFor(x => x.SourceRecordKey)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(512);

            RuleFor(x => x.SourceUrl)
                .MaximumLength(2048);

            RuleFor(x => x.ContentHash)
                .MaximumLength(128);

            RuleFor(x => x)
                .Must(HaveValidSeenDates)
                .WithMessage("LastSeenUtc cannot be earlier than FirstSeenUtc.");
        }

        /// <summary>
        /// Kontrollerar datumintervallet när båda observationstiderna anges.
        /// </summary>
        private static bool HaveValidSeenDates(CreatePartyExternalReferenceRequest request)
        {
            return !request.FirstSeenUtc.HasValue
                || !request.LastSeenUtc.HasValue
                || request.LastSeenUtc.Value >= request.FirstSeenUtc.Value;
        }
    }
}