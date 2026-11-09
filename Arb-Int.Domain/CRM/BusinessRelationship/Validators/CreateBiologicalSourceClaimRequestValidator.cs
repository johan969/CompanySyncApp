using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som skapar biologiska source-claims.
    /// </summary>
    public sealed class CreateBiologicalSourceClaimRequestValidator : AbstractValidator<CreateBiologicalSourceClaimRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya biologiska source-claims.
        /// </summary>
        public CreateBiologicalSourceClaimRequestValidator()
        {
            RuleFor(x => x.DeclaredName)
                .NotEmpty()
                .Must(value => !string.IsNullOrWhiteSpace(value))
                .MaximumLength(512);

            RuleFor(x => x.BiologicalSourceId)
                .NotEqual(Guid.Empty)
                .When(x => x.BiologicalSourceId.HasValue);

            RuleFor(x => x.RelationshipKind)
                .IsInEnum();

            RuleFor(x => x.MappingStatus)
                .IsInEnum();

            RuleFor(x => x.MappingConfidence)
                .InclusiveBetween(0m, 1m)
                .When(x => x.MappingConfidence.HasValue);

            RuleFor(x => x.VerificationStatus)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .MaximumLength(4096);

            RuleFor(x => x)
                .Must(HaveValidObservedDates)
                .WithMessage("LastObservedUtc cannot be earlier than FirstObservedUtc.");

            RuleFor(x => x)
                .Must(HaveValidVerifiedDate)
                .WithMessage("LastVerifiedUtc cannot be earlier than FirstObservedUtc.");

            RuleFor(x => x)
                .Must(HaveValidConfirmedMapping)
                .WithMessage("A confirmed mapping requires BiologicalSourceId.");
        }

        /// <summary>
        /// Kontrollerar att observationsintervallet är giltigt när båda tiderna anges.
        /// </summary>
        private static bool HaveValidObservedDates(CreateBiologicalSourceClaimRequest request)
        {
            return !request.FirstObservedUtc.HasValue
                || !request.LastObservedUtc.HasValue
                || request.LastObservedUtc.Value >= request.FirstObservedUtc.Value;
        }

        /// <summary>
        /// Kontrollerar att verifieringstiden inte ligger före första observationen.
        /// </summary>
        private static bool HaveValidVerifiedDate(CreateBiologicalSourceClaimRequest request)
        {
            return !request.FirstObservedUtc.HasValue
                || !request.LastVerifiedUtc.HasValue
                || request.LastVerifiedUtc.Value >= request.FirstObservedUtc.Value;
        }

        /// <summary>
        /// Kontrollerar databasregeln för bekräftade mappings.
        /// </summary>
        private static bool HaveValidConfirmedMapping(CreateBiologicalSourceClaimRequest request)
        {
            return request.MappingStatus != ClaimMappingStatus.Confirmed
                || request.BiologicalSourceId.HasValue;
        }
    }
}