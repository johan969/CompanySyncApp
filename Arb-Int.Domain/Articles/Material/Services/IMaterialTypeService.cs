using FluentResults;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Läser material type reference-data.
    /// </summary>
    public interface IMaterialTypeService
    {
        /// <summary>
        /// Hämtar en material type.
        /// </summary>
        Task<Result<MaterialTypeResponse>> GetByIdAsync(int materialTypeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar material types.
        /// </summary>
        Task<Result<PagedResult<MaterialTypeResponse>>> QueryAsync(MaterialTypeQueryRequest request, CancellationToken cancellationToken = default);
    }
}
