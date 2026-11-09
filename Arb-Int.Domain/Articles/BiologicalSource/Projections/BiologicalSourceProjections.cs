using System.Linq.Expressions;

namespace Arb_Int.Domain.Articles.Projections
{
    /// <summary>
    /// Innehåller återanvändbar EF-projektion för biologiska källor.
    /// </summary>
    public static class BiologicalSourceProjections
    {
        /// <summary>
        /// Projekterar en biologisk källa till response-modellen.
        /// </summary>
        public static Expression<Func<BiologicalSource, BiologicalSourceResponse>> Details => source => new BiologicalSourceResponse
        {
            Id = source.Id,
            Name = source.Name,
            Slug = source.Slug,
            Description = source.Description,
            RecordKind = source.RecordKind,
            DeclaredRank = source.DeclaredRank,
            TaxonId = source.TaxonId,
            ParentBiologicalSourceId = source.ParentBiologicalSourceId,
            ParentBiologicalSourceName = source.ParentBiologicalSource == null
                ? null
                : source.ParentBiologicalSource.Name,
            RegulatoryRisk = source.RegulatoryRisk
        };
    }
}
