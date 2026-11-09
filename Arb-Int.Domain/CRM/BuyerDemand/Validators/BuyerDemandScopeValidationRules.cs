using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Innehåller gemensamma valideringsregler för BuyerDemand-scope.
    /// </summary>
    internal static class BuyerDemandScopeValidationRules
    {
        /// <summary>
        /// Lägger till regler som motsvarar scope-modellens target- och quantity-constraints.
        /// </summary>
        public static void AddRules<T>(
            AbstractValidator<T> validator,
            Func<T, Guid?> biologicalSourceClaimId,
            Func<T, Guid?> materialId,
            Func<T, int?> materialTypeId,
            Func<T, Guid?> intendedUseCharacteristicId,
            Func<T, string?> declaredFormat,
            Func<T, string?> intendedUseText,
            Func<T, decimal?> typicalQuantity,
            Func<T, int?> typicalQuantityUnitId)
        {
            validator.RuleFor(x => x)
                .Must(x =>
                    biologicalSourceClaimId(x).HasValue
                    || materialId(x).HasValue
                    || materialTypeId(x).HasValue
                    || intendedUseCharacteristicId(x).HasValue
                    || !string.IsNullOrWhiteSpace(declaredFormat(x))
                    || !string.IsNullOrWhiteSpace(intendedUseText(x)))
                .WithMessage("At least one BuyerDemand scope target must be specified.");

            validator.RuleFor(x => x)
                .Must(x =>
                {
                    var quantity = typicalQuantity(x);
                    var unitId = typicalQuantityUnitId(x);

                    return (!quantity.HasValue && !unitId.HasValue)
                        || (quantity.HasValue && quantity.Value > 0 && unitId.HasValue);
                })
                .WithMessage("TypicalQuantity and TypicalQuantityUnitId must be supplied together and quantity must be greater than zero.");
        }
    }
}