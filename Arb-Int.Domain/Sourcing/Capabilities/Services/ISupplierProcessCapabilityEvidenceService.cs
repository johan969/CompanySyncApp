using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar källobservationer som stöder en supplier process capability.
    /// </summary>
    public interface ISupplierProcessCapabilityEvidenceService
    {
        /// <summary>
        /// Skapar eller uppdaterar evidens utifrån capability, source system och source record key.
        /// </summary>
        Task<Result<SupplierProcessCapabilityResponse>> UpsertAsync(Guid supplierProcessCapabilityId, UpsertSupplierProcessCapabilityEvidenceRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en felaktig evidenspost utan att ta bort capabilityn eller källsystemets data.
        /// </summary>
        Task<Result<SupplierProcessCapabilityResponse>> DeleteAsync(Guid supplierProcessCapabilityEvidenceId, uint supplierProcessCapabilityRowVersion, CancellationToken cancellationToken = default);
    }
}
