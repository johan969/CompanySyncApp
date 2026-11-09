using Arb_Int.Domain.Services.Common;
using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar supply document-roten, subjects, giltighet och status.
    /// </summary>
    public interface ISupplyDocumentService
    {
        /// <summary>
        /// Hämtar ett supply document.
        /// </summary>
        Task<Result<SupplyDocumentResponse>> GetByIdAsync(Guid supplyDocumentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar supply documents.
        /// </summary>
        Task<Result<PagedResult<SupplyDocumentListResponse>>> QueryAsync(SupplyDocumentQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registrerar ett supply document med minst ett subject.
        /// </summary>
        Task<Result<SupplyDocumentResponse>> CreateAsync(CreateSupplyDocumentRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sätter dokumentets giltighetsfönster.
        /// </summary>
        Task<Result<SupplyDocumentResponse>> SetValidityAsync(Guid supplyDocumentId, SetSupplyDocumentValidityRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ändrar dokumentets status.
        /// </summary>
        Task<Result<SupplyDocumentResponse>> ChangeStatusAsync(Guid supplyDocumentId, ChangeSupplyDocumentStatusRequest request, CancellationToken cancellationToken = default);

    }
}
