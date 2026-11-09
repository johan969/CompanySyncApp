using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar redigerbar data för en supplier offering specification.
    /// </summary>
    public sealed class UpdateSupplierOfferingSpecificationRequestValidator : AbstractValidator<UpdateSupplierOfferingSpecificationRequest>
    {
        /// <summary>
        /// Skapar regler för target, värden, verifiering och giltighet.
        /// </summary>
        public UpdateSupplierOfferingSpecificationRequestValidator()
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
        private static void ValidateTarget(SupplierOfferingSpecificationKind kind, Guid? constituentId, Guid? characteristicId, ValidationContext<UpdateSupplierOfferingSpecificationRequest> context)
        {
            if (kind == SupplierOfferingSpecificationKind.Constituent)
            {
                if (!constituentId.HasValue)
                {
                    context.AddFailure(
                        nameof(UpdateSupplierOfferingSpecificationRequest.ConstituentId),
                        "ConstituentId is required when Kind is Constituent.");
                }

                if (characteristicId.HasValue)
                {
                    context.AddFailure(
                        nameof(UpdateSupplierOfferingSpecificationRequest.CharacteristicId),
                        "CharacteristicId must be null when Kind is Constituent.");
                }

                return;
            }

            if (kind == SupplierOfferingSpecificationKind.Characteristic)
            {
                if (!characteristicId.HasValue)
                {
                    context.AddFailure(
                        nameof(UpdateSupplierOfferingSpecificationRequest.CharacteristicId),
                        "CharacteristicId is required when Kind is Characteristic.");
                }

                if (constituentId.HasValue)
                {
                    context.AddFailure(
                        nameof(UpdateSupplierOfferingSpecificationRequest.ConstituentId),
                        "ConstituentId must be null when Kind is Characteristic.");
                }

                return;
            }

            if (constituentId.HasValue || characteristicId.HasValue)
            {
                context.AddFailure(
                    nameof(UpdateSupplierOfferingSpecificationRequest.Kind),
                    "ConstituentId and CharacteristicId must be null for this specification kind.");
            }
        }
    }
}
