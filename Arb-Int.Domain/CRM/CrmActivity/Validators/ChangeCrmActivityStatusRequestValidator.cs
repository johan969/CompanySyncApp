using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar statusändringar för CRM-aktiviteter.
    /// </summary>
    public sealed class ChangeCrmActivityStatusRequestValidator : AbstractValidator<ChangeCrmActivityStatusRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för statusändringar.
        /// </summary>
        public ChangeCrmActivityStatusRequestValidator()
        {
            RuleFor(x => x.Status).IsInEnum();

            RuleFor(x => x)
                .Must(HaveValidCompletedState)
                .WithMessage("CompletedUtc may only be supplied when Status is Completed.");
        }

        /// <summary>
        /// Kontrollerar att CompletedUtc endast används för slutförda aktiviteter.
        /// </summary>
        private static bool HaveValidCompletedState(ChangeCrmActivityStatusRequest request)
        {
            return request.Status == CrmActivityStatus.Completed || !request.CompletedUtc.HasValue;
        }
    }
}