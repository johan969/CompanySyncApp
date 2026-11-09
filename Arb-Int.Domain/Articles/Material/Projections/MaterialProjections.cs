using System.Linq.Expressions;

namespace Arb_Int.Domain.Articles.Projections
{
    /// <summary>
    /// Innehåller återanvändbar EF-projektion för materials.
    /// </summary>
    public static class MaterialProjections
    {
        /// <summary>
        /// Projekterar ett material till response-modellen.
        /// </summary>
        public static Expression<Func<Material, MaterialResponse>> Details => material => new MaterialResponse
        {
            Id = material.Id,
            Name = material.Name,
            Slug = material.Slug,
            Description = material.Description,
            MaterialTypeId = material.MaterialTypeId,
            MaterialTypeName = material.MaterialType.Name,
            IsRawMaterial = material.MaterialType.IsRawMaterial,
            IsActive = material.IsActive
        };
    }
}
