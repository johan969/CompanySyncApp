using FluentResults;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Läser reference-data för biologiska källor.
    /// </summary>
    public interface IBiologicalSourceService
    {
        /// <summary>
        /// Hämtar en biologisk källa.
        /// </summary>
        Task<Result<BiologicalSourceResponse>> GetByIdAsync(Guid biologicalSourceId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar biologiska källor.
        /// </summary>
        Task<Result<PagedResult<BiologicalSourceResponse>>> QueryAsync(BiologicalSourceQueryRequest request, CancellationToken cancellationToken = default);
    }
}
