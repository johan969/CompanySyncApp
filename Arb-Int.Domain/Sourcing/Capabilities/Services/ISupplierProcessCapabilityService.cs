using Arb_Int.Domain.Services.Common;
using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar supplier process capability-roten, mapping, status och verifiering.
    /// </summary>
    public interface ISupplierProcessCapabilityService
    {
        /// <summary>
        /// Hämtar en capability med capability-ägda scopes och evidens.
        /// </summary>
        Task<Result<SupplierProcessCapabilityResponse>> GetByIdAsync(Guid supplierProcessCapabilityId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar capabilities över samtliga supplier-relationer
        /// som ägs av organisationen.
        /// </summary>
        Task<Result<PagedResult<SupplierProcessCapabilityListResponse>>> QueryByOrganizationAsync(Guid owningOrganizationId, SupplierProcessCapabilityQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar capabilities för en supplier-relation.
        /// </summary>
        Task<Result<PagedResult<SupplierProcessCapabilityListResponse>>> QueryAsync(Guid businessRelationshipId, SupplierProcessCapabilityQueryRequest request, CancellationToken cancellationToken = default);
        /// <summary>
        /// Skapar en observerad och ännu omappad capability inom en supplier-relation.
        /// </summary>
        Task<Result<SupplierProcessCapabilityResponse>> CreateAsync(Guid businessRelationshipId, CreateSupplierProcessCapabilityRequest request, CancellationToken cancellationToken = default);
        /// <summary>
        /// Uppdaterar deklarerad metadata utan att ändra identitet, mapping, status eller verifiering.
        /// </summary>
        Task<Result<SupplierProcessCapabilityResponse>> UpdateAsync(Guid supplierProcessCapabilityId, UpdateSupplierProcessCapabilityRequest request, CancellationToken cancellationToken = default);
        /// <summary>
        /// Sätter process- och materialtypmapping explicit.
        /// </summary>
        Task<Result<SupplierProcessCapabilityResponse>> SetMappingAsync(Guid supplierProcessCapabilityId, SetSupplierProcessCapabilityMappingRequest request, CancellationToken cancellationToken = default);
        /// <summary>
        /// Ändrar capabilityns aktuella sourcing-status.
        /// </summary>
        Task<Result<SupplierProcessCapabilityResponse>> ChangeStatusAsync(Guid supplierProcessCapabilityId, ChangeSupplierProcessCapabilityStatusRequest request, CancellationToken cancellationToken = default);
        /// <summary>
        /// Sätter capabilityns verification-status och registrerar vid behov en ny verifieringstid.
        /// </summary>
        Task<Result<SupplierProcessCapabilityResponse>> SetVerificationAsync(Guid supplierProcessCapabilityId, SetSupplierProcessCapabilityVerificationRequest request, CancellationToken cancellationToken = default);
    }
}
