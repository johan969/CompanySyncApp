using System.Linq.Expressions;

namespace Arb_Int.Domain.Articles.Projections
{
    /// <summary>
    /// Innehåller återanvändbar EF-projektion för material types.
    /// </summary>
    public static class MaterialTypeProjections
    {
        /// <summary>
        /// Projekterar en material type till response-modellen.
        /// </summary>
        public static Expression<Func<MaterialType, MaterialTypeResponse>> Details => materialType => new MaterialTypeResponse
        {
            Id = materialType.Id,
            Code = materialType.Code,
            Name = materialType.Name,
            Description = materialType.Description,
            SortOrder = materialType.SortOrder,
            IsRawMaterial = materialType.IsRawMaterial,
            ParentMaterialTypeId = materialType.ParentMaterialTypeId,
            ParentMaterialTypeName = materialType.ParentMaterialType == null
                ? null
                : materialType.ParentMaterialType.Name,
            IsActive = materialType.IsActive
        };
    }
}
