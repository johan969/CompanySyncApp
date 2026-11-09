using System.Linq.Expressions;

namespace Arb_Int.Domain.Sourcing.Projections
{
    /// <summary>
    /// Innehåller återanvändbar EF-projektion för document types.
    /// </summary>
    public static class DocumentTypeProjections
    {
        /// <summary>
        /// Projekterar en document type till response-modellen.
        /// </summary>
        public static Expression<Func<DocumentType, DocumentTypeResponse>> Details => documentType => new DocumentTypeResponse
        {
            Id = documentType.Id,
            Code = documentType.Code,
            Name = documentType.Name,
            Description = documentType.Description,
            Category = documentType.Category,
            IsLotSpecificByDefault = documentType.IsLotSpecificByDefault,
            CanExpire = documentType.CanExpire,
            IsActive = documentType.IsActive,
            RowVersion = documentType.RowVersion
        };
    }
}
