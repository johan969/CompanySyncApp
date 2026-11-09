using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar biologiska avgränsningar för supplier process capabilities.
    /// </summary>
    public interface ISupplierProcessCapabilityBiologicalSourceScopeService
    {
        /// <summary>
        /// Lägger till ett biologiskt scope och returnerar den uppdaterade capabilityn.
        /// </summary>
        Task<Result<SupplierProcessCapabilityResponse>> AddAsync(Guid supplierProcessCapabilityId, CreateSupplierProcessCapabilityBiologicalSourceScopeRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar anteckningar utan att byta claim-identitet.
        /// </summary>
        Task<Result<SupplierProcessCapabilityResponse>> UpdateAsync(Guid supplierProcessCapabilityBiologicalSourceScopeId, UpdateSupplierProcessCapabilityBiologicalSourceScopeRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort ett biologiskt scope utan att ta bort CRM-claimet.
        /// </summary>
        Task<Result<SupplierProcessCapabilityResponse>> DeleteAsync(Guid supplierProcessCapabilityBiologicalSourceScopeId, uint supplierProcessCapabilityRowVersion, CancellationToken cancellationToken = default);
    }
}
