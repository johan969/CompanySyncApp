using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar biologiska CRM-claims som ett supplier material offering bygger på.
    /// </summary>
    public interface ISupplierMaterialOfferingBiologicalSourceService
    {
        /// <summary>
        /// Lägger till en biologisk source-koppling och returnerar det uppdaterade offeringet.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> AddAsync(Guid supplierMaterialOfferingId, CreateSupplierMaterialOfferingBiologicalSourceRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar roll, proportion och anteckningar utan att byta claim-identitet.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> UpdateAsync(Guid supplierMaterialOfferingBiologicalSourceId, UpdateSupplierMaterialOfferingBiologicalSourceRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en biologisk source-koppling utan att ta bort CRM-claimet.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> DeleteAsync(Guid supplierMaterialOfferingBiologicalSourceId, uint supplierMaterialOfferingRowVersion, CancellationToken cancellationToken = default);
    }
}
