using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som uppdaterar CRM-aktiviteter.
    /// </summary>
    public sealed class UpdateCrmActivityRequestValidator : AbstractValidator<UpdateCrmActivityRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för CRM-aktivitetsuppdateringar.
        /// </summary>
        public UpdateCrmActivityRequestValidator()
        {
            RuleFor(x => x.ActivityType).IsInEnum();
            RuleFor(x => x.Direction).IsInEnum();
            RuleFor(x => x.Subject).NotEmpty().Must(value => !string.IsNullOrWhiteSpace(value)).MaximumLength(512);
            RuleFor(x => x.Description).NotNull().MaximumLength(8192);
            RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
            RuleFor(x => x.ExternalSystem).MaximumLength(100);
            RuleFor(x => x.ExternalRecordId).MaximumLength(512);

            RuleFor(x => x)
                .Must(HaveCompleteExternalReference)
                .WithMessage("ExternalSystem and ExternalRecordId must either both be set or both be null.");
        }

        /// <summary>
        /// Kontrollerar att extern systemreferens anges som ett komplett par.
        /// </summary>
        private static bool HaveCompleteExternalReference(UpdateCrmActivityRequest request)
        {
            return string.IsNullOrWhiteSpace(request.ExternalSystem) == string.IsNullOrWhiteSpace(request.ExternalRecordId);
        }
    }
}