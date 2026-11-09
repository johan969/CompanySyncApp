using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar en ny certification assertion och dess subject-shape.
    /// </summary>
    public sealed class CreateSupplyCertificationRequestValidator : AbstractValidator<CreateSupplyCertificationRequest>
    {
        /// <summary>
        /// Skapar regler för scheme, exakt ett subject, scope och validity.
        /// </summary>
        public CreateSupplyCertificationRequestValidator()
        {
            RuleFor(x => x.CertificationSchemeId).GreaterThan(0);
            RuleFor(x => x.Scope).IsInEnum();

            RuleFor(x => x)
                .Must(HasExactlyOneSubject)
                .WithMessage("Exactly one SupplyCertification subject must be specified.");

            RuleFor(x => x.ValidToUtc)
                .GreaterThanOrEqualTo(x => x.ValidFromUtc!.Value)
                .When(x => x.ValidFromUtc.HasValue && x.ValidToUtc.HasValue);
        }

        /// <summary>
        /// Kontrollerar att exakt ett av de fem subject-fälten är satt.
        /// </summary>
        private static bool HasExactlyOneSubject(CreateSupplyCertificationRequest request)
        {
            var count = 0;
            if (request.CertifiedBusinessRelationshipId.HasValue) count++;
            if (request.SupplierMaterialOfferingId.HasValue) count++;
            if (request.SupplierProcessCapabilityId.HasValue) count++;
            if (request.InventoryLotId.HasValue) count++;
            if (request.ShipmentId.HasValue) count++;
            return count == 1;
        }
    }
}
