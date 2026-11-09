using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar process- och materialtypmapping för en supplier process capability.
    /// </summary>
    public sealed class SetSupplierProcessCapabilityMappingRequestValidator : AbstractValidator<SetSupplierProcessCapabilityMappingRequest>
    {
        /// <summary>
        /// Skapar regler för mapping-status och confidence.
        /// </summary>
        public SetSupplierProcessCapabilityMappingRequestValidator()
        {
            RuleFor(x => x.MappingStatus).IsInEnum();
            RuleFor(x => x.MappingConfidence).InclusiveBetween(0m, 1m).When(x => x.MappingConfidence.HasValue);
            RuleFor(x => x.ProcessTypeId).NotNull().When(x => x.MappingStatus == SourcingMappingStatus.Confirmed).WithMessage("ProcessTypeId is required when MappingStatus is Confirmed.");
        }
    }
}
