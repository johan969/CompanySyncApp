using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar tidsbegränsad och kvantifierad availability för supplier material offerings.
    /// </summary>
    public interface ISupplierOfferingAvailabilityService
    {
        /// <summary>
        /// Skapar availability för ett offering och returnerar det uppdaterade offeringet.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> CreateAsync(Guid supplierMaterialOfferingId, CreateSupplierOfferingAvailabilityRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar availability-data utan att byta offering eller inventory lot.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> UpdateAsync(Guid supplierOfferingAvailabilityId, UpdateSupplierOfferingAvailabilityRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en availability-post som korrigering.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> DeleteAsync(Guid supplierOfferingAvailabilityId, uint rowVersion, uint supplierMaterialOfferingRowVersion, CancellationToken cancellationToken = default);
    }
}
