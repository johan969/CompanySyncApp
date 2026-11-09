using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar characteristic-taggar för supplier material offerings.
    /// </summary>
    public interface ISupplierMaterialOfferingCharacteristicService
    {
        /// <summary>
        /// Lägger till en characteristic och returnerar det uppdaterade offeringet.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> AddAsync(Guid supplierMaterialOfferingId, CreateSupplierMaterialOfferingCharacteristicRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en characteristic-koppling utan att ta bort masterdata-posten.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> DeleteAsync(Guid supplierMaterialOfferingCharacteristicId, uint supplierMaterialOfferingRowVersion, CancellationToken cancellationToken = default);
    }
}
