using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som skapar CRM-aktiviteter.
    /// </summary>
    public sealed class CreateCrmActivityRequestValidator : AbstractValidator<CreateCrmActivityRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya CRM-aktiviteter.
        /// </summary>
        public CreateCrmActivityRequestValidator()
        {
            RuleFor(x => x.RegardingPartyId).NotEmpty();
            RuleFor(x => x.BusinessRelationshipId).NotEqual(Guid.Empty).When(x => x.BusinessRelationshipId.HasValue);
            RuleFor(x => x.ActivityType).IsInEnum();
            RuleFor(x => x.Status).IsInEnum();
            RuleFor(x => x.Direction).IsInEnum();
            RuleFor(x => x.Subject).NotEmpty().Must(value => !string.IsNullOrWhiteSpace(value)).MaximumLength(512);
            RuleFor(x => x.Description).NotNull().MaximumLength(8192);
            RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
            RuleFor(x => x.ExternalSystem).MaximumLength(100);
            RuleFor(x => x.ExternalRecordId).MaximumLength(512);

            RuleFor(x => x)
                .Must(HaveCompleteExternalReference)
                .WithMessage("ExternalSystem and ExternalRecordId must either both be set or both be null.");

            RuleFor(x => x)
                .Must(HaveValidCompletedState)
                .WithMessage("CompletedUtc may only be supplied when Status is Completed.");
        }

        /// <summary>
        /// Kontrollerar att extern systemreferens anges som ett komplett par.
        /// </summary>
        private static bool HaveCompleteExternalReference(CreateCrmActivityRequest request)
        {
            return string.IsNullOrWhiteSpace(request.ExternalSystem) == string.IsNullOrWhiteSpace(request.ExternalRecordId);
        }

        /// <summary>
        /// Kontrollerar att CompletedUtc endast används för slutförda aktiviteter.
        /// </summary>
        private static bool HaveValidCompletedState(CreateCrmActivityRequest request)
        {
            return request.Status == CrmActivityStatus.Completed || !request.CompletedUtc.HasValue;
        }
    }
}