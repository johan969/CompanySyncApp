using FluentResults;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Läser material reference-data.
    /// </summary>
    public interface IMaterialService
    {
        /// <summary>
        /// Hämtar ett material.
        /// </summary>
        Task<Result<MaterialResponse>> GetByIdAsync(Guid materialId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar materials.
        /// </summary>
        Task<Result<PagedResult<MaterialResponse>>> QueryAsync(MaterialQueryRequest request, CancellationToken cancellationToken = default);
    }
}
