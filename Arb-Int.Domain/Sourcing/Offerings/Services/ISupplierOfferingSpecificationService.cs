using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar mätbara och semantiska specifications för supplier material offerings.
    /// </summary>
    public interface ISupplierOfferingSpecificationService
    {
        /// <summary>
        /// Skapar en specification och returnerar det uppdaterade offeringet.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> CreateAsync(Guid supplierMaterialOfferingId, CreateSupplierOfferingSpecificationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar en specification och concurrency-skyddar både raden och offering-rooten.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> UpdateAsync(Guid supplierOfferingSpecificationId, UpdateSupplierOfferingSpecificationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en specification som korrigering.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> DeleteAsync(Guid supplierOfferingSpecificationId, uint rowVersion, uint supplierMaterialOfferingRowVersion, CancellationToken cancellationToken = default);
    }
}
