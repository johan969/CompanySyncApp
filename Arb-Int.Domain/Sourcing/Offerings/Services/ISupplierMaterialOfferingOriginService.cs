using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar geografiska origin-kopplingar för supplier material offerings.
    /// </summary>
    public interface ISupplierMaterialOfferingOriginService
    {
        /// <summary>
        /// Lägger till en origin-koppling och returnerar det uppdaterade offeringet.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> AddAsync(Guid supplierMaterialOfferingId, CreateSupplierMaterialOfferingOriginRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar roll, primärstatus och anteckningar utan att byta geografisk plats.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> UpdateAsync(Guid supplierMaterialOfferingOriginId, UpdateSupplierMaterialOfferingOriginRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en origin-koppling utan att ta bort GeographicLocation.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> DeleteAsync(Guid supplierMaterialOfferingOriginId, uint supplierMaterialOfferingRowVersion, CancellationToken cancellationToken = default);
    }
}
