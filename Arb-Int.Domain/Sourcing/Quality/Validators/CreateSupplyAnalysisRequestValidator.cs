using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar kärnreferenser för en ny supply analysis.
    /// </summary>
    public sealed class CreateSupplyAnalysisRequestValidator : AbstractValidator<CreateSupplyAnalysisRequest>
    {
        /// <summary>
        /// Skapar regeln att minst offering eller inventory lot måste anges.
        /// </summary>
        public CreateSupplyAnalysisRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => x.SupplierMaterialOfferingId.HasValue || x.InventoryLotId.HasValue)
                .WithMessage("SupplierMaterialOfferingId or InventoryLotId must be specified.");
        }
    }
}
