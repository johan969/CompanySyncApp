using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Innehåller gemensamma valideringsregler för RFQ-rader.
/// </summary>
internal static class BuyerRfqLineValidationRules
{
    /// <summary>
    /// Lägger till regler som motsvarar BuyerRfqLine-modellens databasconstraints.
    /// </summary>
    public static void AddRules<T>(
        AbstractValidator<T> validator,
        Func<T, int> lineNumber,
        Func<T, string> name,
        Func<T, string> description,
        Func<T, string?> buyerLineReference,
        Func<T, string?> buyerOriginalText,
        Func<T, Guid?> materialId,
        Func<T, int?> materialTypeId,
        Func<T, Guid?> intendedUseCharacteristicId,
        Func<T, decimal> requiredQuantity,
        Func<T, int> quantityUnitId,
        Func<T, decimal?> lowerTolerance,
        Func<T, decimal?> upperTolerance,
        Func<T, bool> allowMultipleSuppliers,
        Func<T, bool> allowMultipleOrigins,
        Func<T, bool> allowMultipleLots,
        Func<T, bool> requireSingleSupplier,
        Func<T, bool> requireSingleOrigin,
        Func<T, bool> requireSingleLot,
        Func<T, int?> maximumSupplierCount,
        Func<T, string?> internalNotes)
    {
        validator.RuleFor(x => lineNumber(x)).GreaterThan(0);
        validator.RuleFor(x => name(x)).NotEmpty().Must(x => !string.IsNullOrWhiteSpace(x));
        validator.RuleFor(x => description(x)).NotNull();
        validator.RuleFor(x => buyerLineReference(x)).MaximumLength(255);
        validator.RuleFor(x => buyerOriginalText(x)).MaximumLength(8000);
        validator.RuleFor(x => materialId(x)).NotEqual(Guid.Empty).When(x => materialId(x).HasValue);
        validator.RuleFor(x => materialTypeId(x)).GreaterThan(0).When(x => materialTypeId(x).HasValue);
        validator.RuleFor(x => intendedUseCharacteristicId(x)).NotEqual(Guid.Empty).When(x => intendedUseCharacteristicId(x).HasValue);
        validator.RuleFor(x => requiredQuantity(x)).GreaterThan(0);
        validator.RuleFor(x => quantityUnitId(x)).GreaterThan(0);
        validator.RuleFor(x => lowerTolerance(x)).InclusiveBetween(0m, 100m).When(x => lowerTolerance(x).HasValue);
        validator.RuleFor(x => upperTolerance(x)).InclusiveBetween(0m, 100m).When(x => upperTolerance(x).HasValue);
        validator.RuleFor(x => maximumSupplierCount(x)).GreaterThan(0).When(x => maximumSupplierCount(x).HasValue);
        validator.RuleFor(x => internalNotes(x)).MaximumLength(4096);

        validator.RuleFor(x => x)
            .Must(x => !(allowMultipleSuppliers(x) && requireSingleSupplier(x)))
            .WithMessage("AllowMultipleSuppliers and RequireSingleSupplier cannot both be true.");

        validator.RuleFor(x => x)
            .Must(x => !(allowMultipleOrigins(x) && requireSingleOrigin(x)))
            .WithMessage("AllowMultipleOrigins and RequireSingleOrigin cannot both be true.");

        validator.RuleFor(x => x)
            .Must(x => !(allowMultipleLots(x) && requireSingleLot(x)))
            .WithMessage("AllowMultipleLots and RequireSingleLot cannot both be true.");
    }
}