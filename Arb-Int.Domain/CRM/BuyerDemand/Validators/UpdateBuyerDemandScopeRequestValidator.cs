using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som uppdaterar BuyerDemand-scope.
    /// </summary>
    public sealed class UpdateBuyerDemandScopeRequestValidator : AbstractValidator<UpdateBuyerDemandScopeRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för scope-uppdateringar.
        /// </summary>
        public UpdateBuyerDemandScopeRequestValidator()
        {
            RuleFor(x => x.BiologicalSourceClaimId).NotEqual(Guid.Empty).When(x => x.BiologicalSourceClaimId.HasValue);
            RuleFor(x => x.MaterialId).NotEqual(Guid.Empty).When(x => x.MaterialId.HasValue);
            RuleFor(x => x.IntendedUseCharacteristicId).NotEqual(Guid.Empty).When(x => x.IntendedUseCharacteristicId.HasValue);
            RuleFor(x => x.DeclaredFormat).MaximumLength(255);
            RuleFor(x => x.IntendedUseText).MaximumLength(2048);
            RuleFor(x => x.RecurrenceDescription).MaximumLength(1000);
            RuleFor(x => x.RowVersion).NotEqual(0u);

            BuyerDemandScopeValidationRules.AddRules(
                this,
                x => x.BiologicalSourceClaimId,
                x => x.MaterialId,
                x => x.MaterialTypeId,
                x => x.IntendedUseCharacteristicId,
                x => x.DeclaredFormat,
                x => x.IntendedUseText,
                x => x.TypicalQuantity,
                x => x.TypicalQuantityUnitId);
        }
    }
}