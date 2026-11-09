using Arb_Int.Domain.Services.Common;
using FluentResults;

namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Hanterar supplier material offering-roten, materialmapping och kommersiell observationsstatus.
/// </summary>
public interface ISupplierMaterialOfferingService
{
    /// <summary>
    /// Hämtar ett offering med offering-ägda child-data.
    /// </summary>
    Task<Result<SupplierMaterialOfferingResponse>> GetByIdAsync(Guid supplierMaterialOfferingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Söker, filtrerar, sorterar och paginerar offerings över samtliga supplier-relationer
    /// som ägs av organisationen.
    /// </summary>
    Task<Result<PagedResult<SupplierMaterialOfferingListResponse>>> QueryByOrganizationAsync(Guid owningOrganizationId, SupplierMaterialOfferingQueryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Söker, filtrerar, sorterar och paginerar offerings för en supplier-relation.
    /// </summary>
    Task<Result<PagedResult<SupplierMaterialOfferingListResponse>>> QueryAsync(Guid businessRelationshipId, SupplierMaterialOfferingQueryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett observerat offering inom en supplier-relation.
    /// </summary>
    Task<Result<SupplierMaterialOfferingResponse>> CreateAsync(Guid businessRelationshipId, CreateSupplierMaterialOfferingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar offeringens deklarerade metadata utan att ändra identitet, mapping eller status.
    /// </summary>
    Task<Result<SupplierMaterialOfferingResponse>> UpdateAsync(Guid supplierMaterialOfferingId, UpdateSupplierMaterialOfferingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sätter normaliserad materialtyp och kanonisk materialmapping explicit.
    /// </summary>
    Task<Result<SupplierMaterialOfferingResponse>> SetMappingAsync(Guid supplierMaterialOfferingId, SetSupplierMaterialOfferingMappingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ändrar offeringens aktuella kommersiella observationsstatus.
    /// </summary>
    Task<Result<SupplierMaterialOfferingResponse>> ChangeStatusAsync(Guid supplierMaterialOfferingId, ChangeSupplierMaterialOfferingStatusRequest request, CancellationToken cancellationToken = default);
}
