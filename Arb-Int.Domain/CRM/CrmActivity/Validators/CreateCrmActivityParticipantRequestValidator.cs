using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som lägger till CRM-aktivitetsdeltagare.
    /// </summary>
    public sealed class CreateCrmActivityParticipantRequestValidator : AbstractValidator<CreateCrmActivityParticipantRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya aktivitetsdeltagare.
        /// </summary>
        public CreateCrmActivityParticipantRequestValidator()
        {
            RuleFor(x => x.PartyId).NotEmpty();
            RuleFor(x => x.Role).IsInEnum();
        }
    }
}