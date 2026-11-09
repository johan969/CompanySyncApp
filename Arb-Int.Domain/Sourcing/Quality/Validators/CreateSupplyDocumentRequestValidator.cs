using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar kärndata och subjects för ett nytt supply document.
    /// </summary>
    public sealed class CreateSupplyDocumentRequestValidator : AbstractValidator<CreateSupplyDocumentRequest>
    {
        /// <summary>
        /// Skapar regler för document type, subjects och giltighetsfönster.
        /// </summary>
        public CreateSupplyDocumentRequestValidator()
        {
            RuleFor(x => x.DocumentTypeId)
                .GreaterThan(0);

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x)
                .Must(HasAtLeastOneSubject)
                .WithMessage("At least one document subject must be specified.");

            RuleFor(x => x.ValidToUtc)
                .GreaterThanOrEqualTo(x => x.ValidFromUtc!.Value)
                .When(x => x.ValidFromUtc.HasValue && x.ValidToUtc.HasValue);
        }

        /// <summary>
        /// Kontrollerar att dokumentet har minst ett subject enligt databasens subject-constraint.
        /// </summary>
        private static bool HasAtLeastOneSubject(CreateSupplyDocumentRequest request)
        {
            return request.SubjectBusinessRelationshipId.HasValue
                || request.SupplierMaterialOfferingId.HasValue
                || request.SupplierProcessCapabilityId.HasValue
                || request.InventoryLotId.HasValue
                || request.ShipmentId.HasValue;
        }
    }
}
