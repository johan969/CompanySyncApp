using Arb_Int.Domain.Services.Common;
using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar supply analysis-roten, status och supporting document.
    /// </summary>
    public interface ISupplyAnalysisService
    {
        /// <summary>
        /// Hämtar en supply analysis med resultat.
        /// </summary>
        Task<Result<SupplyAnalysisResponse>> GetByIdAsync(Guid supplyAnalysisId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar supply analyses.
        /// </summary>
        Task<Result<PagedResult<SupplyAnalysisListResponse>>> QueryAsync(SupplyAnalysisQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar en draft analysis för offering, inventory lot eller båda.
        /// </summary>
        Task<Result<SupplyAnalysisResponse>> CreateAsync(CreateSupplyAnalysisRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sätter eller rensar supporting supply document.
        /// </summary>
        Task<Result<SupplyAnalysisResponse>> SetDocumentAsync(Guid supplyAnalysisId, SetSupplyAnalysisDocumentRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ändrar analysens status.
        /// </summary>
        Task<Result<SupplyAnalysisResponse>> ChangeStatusAsync(Guid supplyAnalysisId, ChangeSupplyAnalysisStatusRequest request, CancellationToken cancellationToken = default);

    }
}
