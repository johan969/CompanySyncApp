using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar grunddata för en ny supplier process capability.
    /// </summary>
    public sealed class CreateSupplierProcessCapabilityRequestValidator : AbstractValidator<CreateSupplierProcessCapabilityRequest>
    {
        /// <summary>
        /// Skapar regler för identitet, deklarerad metadata och observationsdatum.
        /// </summary>
        public CreateSupplierProcessCapabilityRequestValidator()
        {
            RuleFor(x => x.CapabilityKey).NotEmpty().MaximumLength(128);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(512);
            RuleFor(x => x.DeclaredProcessName).NotEmpty().MaximumLength(512);
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.Notes).MaximumLength(4096);
            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.FirstObservedUtc.HasValue && request.LastObservedUtc.HasValue && request.LastObservedUtc.Value < request.FirstObservedUtc.Value)
                {
                    context.AddFailure(nameof(request.LastObservedUtc), "LastObservedUtc cannot be earlier than FirstObservedUtc.");
                }
            });
        }
    }
}
