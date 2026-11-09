using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som ändrar en aktivitetsdeltagares roll.
    /// </summary>
    public sealed class UpdateCrmActivityParticipantRequestValidator : AbstractValidator<UpdateCrmActivityParticipantRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för deltagaruppdateringar.
        /// </summary>
        public UpdateCrmActivityParticipantRequestValidator()
        {
            RuleFor(x => x.Role).IsInEnum();
        }
    }
}