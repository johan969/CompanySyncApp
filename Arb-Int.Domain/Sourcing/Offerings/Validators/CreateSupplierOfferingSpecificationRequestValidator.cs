using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar en ny specification för ett supplier material offering.
    /// </summary>
    public sealed class CreateSupplierOfferingSpecificationRequestValidator : AbstractValidator<CreateSupplierOfferingSpecificationRequest>
    {
        /// <summary>
        /// Skapar regler för target, värden, verifiering och giltighet.
        /// </summary>
        public CreateSupplierOfferingSpecificationRequestValidator()
        {
            RuleFor(x => x.Kind)
                .IsInEnum();

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Description)
                .NotNull();

            RuleFor(x => x.Operator)
                .IsInEnum();

            RuleFor(x => x.Basis)
                .IsInEnum();

            RuleFor(x => x.VerificationState)
                .IsInEnum();

            RuleFor(x => x.TextValue)
                .MaximumLength(2048);

            RuleFor(x => x.AnalysisMethod)
                .MaximumLength(1000);

            RuleFor(x => x.ValidToUtc)
                .GreaterThanOrEqualTo(x => x.ValidFromUtc!.Value)
                .When(x => x.ValidFromUtc.HasValue && x.ValidToUtc.HasValue);

            RuleFor(x => x)
                .Custom(
                    (request, context) =>
                    {
                        ValidateTarget(
                            request.Kind,
                            request.ConstituentId,
                            request.CharacteristicId,
                            context);

                        if (request.MinimumValue.HasValue
                            && request.MaximumValue.HasValue
                            && request.MaximumValue.Value < request.MinimumValue.Value)
                        {
                            context.AddFailure(
                                nameof(request.MaximumValue),
                                "MaximumValue cannot be lower than MinimumValue.");
                        }
                    });
        }

        /// <summary>
        /// Validerar att canonical target följer specificationens Kind.
        /// </summary>
        private static void ValidateTarget(SupplierOfferingSpecificationKind kind, Guid? constituentId, Guid? characteristicId, ValidationContext<CreateSupplierOfferingSpecificationRequest> context)
        {
            if (kind == SupplierOfferingSpecificationKind.Constituent)
            {
                if (!constituentId.HasValue)
                {
                    context.AddFailure(
                        nameof(CreateSupplierOfferingSpecificationRequest.ConstituentId),
                        "ConstituentId is required when Kind is Constituent.");
                }

                if (characteristicId.HasValue)
                {
                    context.AddFailure(
                        nameof(CreateSupplierOfferingSpecificationRequest.CharacteristicId),
                        "CharacteristicId must be null when Kind is Constituent.");
                }

                return;
            }

            if (kind == SupplierOfferingSpecificationKind.Characteristic)
            {
                if (!characteristicId.HasValue)
                {
                    context.AddFailure(
                        nameof(CreateSupplierOfferingSpecificationRequest.CharacteristicId),
                        "CharacteristicId is required when Kind is Characteristic.");
                }

                if (constituentId.HasValue)
                {
                    context.AddFailure(
                        nameof(CreateSupplierOfferingSpecificationRequest.ConstituentId),
                        "ConstituentId must be null when Kind is Characteristic.");
                }

                return;
            }

            if (constituentId.HasValue || characteristicId.HasValue)
            {
                context.AddFailure(
                    nameof(CreateSupplierOfferingSpecificationRequest.Kind),
                    "ConstituentId and CharacteristicId must be null for this specification kind.");
            }
        }
    }
}
