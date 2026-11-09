using FluentResults;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Läser process type reference-data.
    /// </summary>
    public interface IProcessTypeService
    {
        /// <summary>
        /// Hämtar en process type.
        /// </summary>
        Task<Result<ProcessTypeResponse>> GetByIdAsync(int processTypeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar process types.
        /// </summary>
        Task<Result<PagedResult<ProcessTypeResponse>>> QueryAsync(ProcessTypeQueryRequest request, CancellationToken cancellationToken = default);
    }
}
