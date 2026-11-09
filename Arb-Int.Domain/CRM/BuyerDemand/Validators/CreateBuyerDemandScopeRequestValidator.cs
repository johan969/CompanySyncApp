using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som skapar BuyerDemand-scope.
    /// </summary>
    public sealed class CreateBuyerDemandScopeRequestValidator : AbstractValidator<CreateBuyerDemandScopeRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för nya scopes.
        /// </summary>
        public CreateBuyerDemandScopeRequestValidator()
        {
            RuleFor(x => x.BiologicalSourceClaimId).NotEqual(Guid.Empty).When(x => x.BiologicalSourceClaimId.HasValue);
            RuleFor(x => x.MaterialId).NotEqual(Guid.Empty).When(x => x.MaterialId.HasValue);
            RuleFor(x => x.IntendedUseCharacteristicId).NotEqual(Guid.Empty).When(x => x.IntendedUseCharacteristicId.HasValue);
            RuleFor(x => x.DeclaredFormat).MaximumLength(255);
            RuleFor(x => x.IntendedUseText).MaximumLength(2048);
            RuleFor(x => x.RecurrenceDescription).MaximumLength(1000);

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