using Arb_Int.Domain.Taxonomy;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Innehåller reference-data för en biologisk källa.
    /// </summary>
    public sealed class BiologicalSourceResponse
    {
        /// <summary>
        /// Identifierar den biologiska källan.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Anger källans visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Anger källans stabila slug.
        /// </summary>
        public required string Slug { get; init; }

        /// <summary>
        /// Beskriver källan.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// Anger posttypen.
        /// </summary>
        public BiologicalSourceRecordKind RecordKind { get; init; }

        /// <summary>
        /// Anger den deklarerade taxonomiska nivån.
        /// </summary>
        public TaxonRank? DeclaredRank { get; init; }

        /// <summary>
        /// Identifierar det kopplade taxonet.
        /// </summary>
        public Guid? TaxonId { get; init; }

        /// <summary>
        /// Identifierar överordnad biologisk källa.
        /// </summary>
        public Guid? ParentBiologicalSourceId { get; init; }

        /// <summary>
        /// Anger överordnad källas visningsnamn.
        /// </summary>
        public string? ParentBiologicalSourceName { get; init; }

        /// <summary>
        /// Anger bedömd regulatorisk risk.
        /// </summary>
        public RegulatoryRiskLevel? RegulatoryRisk { get; init; }
    }
}
