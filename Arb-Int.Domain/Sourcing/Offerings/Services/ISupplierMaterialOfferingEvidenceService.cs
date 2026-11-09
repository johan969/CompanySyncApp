using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar källobservationer som stöder att ett supplier material offering finns.
    /// </summary>
    public interface ISupplierMaterialOfferingEvidenceService
    {
        /// <summary>
        /// Skapar eller uppdaterar en evidenspost utifrån offering, source system och source record key.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> UpsertAsync(Guid supplierMaterialOfferingId, UpsertSupplierMaterialOfferingEvidenceRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en felaktig evidenspost utan att ta bort offeringet eller källsystemets data.
        /// </summary>
        Task<Result<SupplierMaterialOfferingResponse>> DeleteAsync(Guid supplierMaterialOfferingEvidenceId, uint supplierMaterialOfferingRowVersion, CancellationToken cancellationToken = default);
    }
}
