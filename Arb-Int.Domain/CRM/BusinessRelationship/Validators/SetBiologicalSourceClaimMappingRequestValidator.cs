using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar ändringar av biologiska source-claim mappings.
    /// </summary>
    public sealed class SetBiologicalSourceClaimMappingRequestValidator : AbstractValidator<SetBiologicalSourceClaimMappingRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för mappingändringar.
        /// </summary>
        public SetBiologicalSourceClaimMappingRequestValidator()
        {
            RuleFor(x => x.BiologicalSourceId)
                .NotEqual(Guid.Empty)
                .When(x => x.BiologicalSourceId.HasValue);

            RuleFor(x => x.MappingStatus)
                .IsInEnum();

            RuleFor(x => x.MappingConfidence)
                .InclusiveBetween(0m, 1m)
                .When(x => x.MappingConfidence.HasValue);

            RuleFor(x => x)
                .Must(x => x.MappingStatus != ClaimMappingStatus.Confirmed || x.BiologicalSourceId.HasValue)
                .WithMessage("A confirmed mapping requires BiologicalSourceId.");
        }
    }
}