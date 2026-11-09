using FluentValidation;

namespace Arb_Int.Domain.Parties.Validation
{
    /// <summary>
    /// Validerar requests som uppdaterar externa Party-referenser.
    /// </summary>
    public sealed class UpdatePartyExternalReferenceRequestValidator : AbstractValidator<UpdatePartyExternalReferenceRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för uppdateringar av externa Party-referenser.
        /// </summary>
        public UpdatePartyExternalReferenceRequestValidator()
        {
            RuleFor(x => x.SourceUrl)
                .MaximumLength(2048);

            RuleFor(x => x.FirstSeenUtc)
                .NotEqual(default(DateTimeOffset));

            RuleFor(x => x.LastSeenUtc)
                .NotEqual(default(DateTimeOffset))
                .GreaterThanOrEqualTo(x => x.FirstSeenUtc);

            RuleFor(x => x.ContentHash)
                .MaximumLength(128);
        }
    }
}