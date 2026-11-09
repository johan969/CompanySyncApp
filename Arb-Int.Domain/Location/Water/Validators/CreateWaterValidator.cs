
using Arb_Int.Domain.Base;
using Arb_Int.Domain.User;
using FluentValidation;

namespace Arb_Int.Domain.Location
{
    public abstract class WaterValidatorBase<T> : AbstractValidatorBase<T>
            where T : class
    {

    }

    public sealed class CreateWaterValidator : WaterValidatorBase<CreateWaterRequest>
    {
        public CreateWaterValidator(ICurrentUserService currentUserService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name can not be empty");
        }
    }
}