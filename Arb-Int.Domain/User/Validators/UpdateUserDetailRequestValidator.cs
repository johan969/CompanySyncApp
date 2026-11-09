using FluentValidation;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Validerar grundformen för en användarprofiluppdatering.
    /// </summary>
    public sealed class UpdateUserDetailRequestValidator : AbstractValidator<UpdateUserDetailRequest>
    {
        /// <summary>
        /// Skapar valideringsreglerna för profiluppdateringen.
        /// </summary>
        public UpdateUserDetailRequestValidator()
        {
            RuleFor(x => x.AuthEmailContactPointId)
                .Must(x => !x.HasValue || x.Value != Guid.Empty);
        }
    }
}
