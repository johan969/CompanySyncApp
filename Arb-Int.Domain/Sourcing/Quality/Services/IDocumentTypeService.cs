using Arb_Int.Domain.Services.Common;
using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar document type reference-data.
    /// </summary>
    public interface IDocumentTypeService
    {
        /// <summary>
        /// Hämtar en document type.
        /// </summary>
        Task<Result<DocumentTypeResponse>> GetByIdAsync(int documentTypeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar document types.
        /// </summary>
        Task<Result<PagedResult<DocumentTypeResponse>>> QueryAsync(DocumentTypeQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar en ny document type med immutable code.
        /// </summary>
        Task<Result<DocumentTypeResponse>> CreateAsync(CreateDocumentTypeRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar document typens redigerbara metadata utan att ändra code.
        /// </summary>
        Task<Result<DocumentTypeResponse>> UpdateAsync(int documentTypeId, UpdateDocumentTypeRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktiverar eller inaktiverar document typen utan fysisk delete.
        /// </summary>
        Task<Result<DocumentTypeResponse>> SetActiveAsync(int documentTypeId, SetDocumentTypeActiveRequest request, CancellationToken cancellationToken = default);
    }
}
