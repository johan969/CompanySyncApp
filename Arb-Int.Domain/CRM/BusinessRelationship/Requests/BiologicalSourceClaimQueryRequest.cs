using Arb_Int.Domain.Articles;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för biologiska source-claims.
    /// </summary>
    public sealed class BiologicalSourceClaimQueryRequest
    {
        /// <summary>
        /// Fritext som matchas mot deklarerat namn, anteckningar och mappad biologisk källa.
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Begränsar resultatet till en normaliserad biologisk källa.
        /// <para>Matchningen är exakt. En claim mappad till genusnivå matchar inte
        /// ett artfilter och omvänt.</para>
        /// </summary>
        public Guid? BiologicalSourceId { get; init; }

        /// <summary>
        /// Begränsar en organisationsvid sökning till en enskild affärsrelation.
        /// <para>Ignoreras när sökningen redan är scopad till en relation via routen.</para>
        /// </summary>
        public Guid? BusinessRelationshipId { get; init; }

        /// <summary>
        /// Begränsar en organisationsvid sökning till en affärsrelationstyp, exempelvis "supplier".
        /// </summary>
        public string? BusinessRelationshipTypeCode { get; init; }

        /// <summary>
        /// Begränsar resultatet till en relationstyp, exempelvis Cultivates eller Supplies.
        /// </summary>
        public BiologicalSourceRelationshipKind? RelationshipKind { get; init; }

        /// <summary>
        /// Begränsar resultatet till en mapping-status.
        /// </summary>
        public ClaimMappingStatus? MappingStatus { get; init; }

        /// <summary>
        /// Begränsar resultatet till en verification-status.
        /// </summary>
        public ClaimVerificationStatus? VerificationStatus { get; init; }

        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public BiologicalSourceClaimSortField SortBy { get; init; } = BiologicalSourceClaimSortField.RelationshipKind;

        /// <summary>
        /// Anger sorteringsriktning.
        /// </summary>
        public SortDirection SortDirection { get; init; } = SortDirection.Ascending;

        /// <summary>
        /// Sidnummer med start på 1.
        /// </summary>
        public int Page { get; init; } = 1;

        /// <summary>
        /// Maximalt antal poster per sida.
        /// </summary>
        public int PageSize { get; init; } = 50;
    }

    /// <summary>
    /// Anger tillgängliga sorteringsfält för biologiska source-claims.
    /// </summary>
    public enum BiologicalSourceClaimSortField
    {
        /// <summary>
        /// Sorterar på relationstyp och därefter deklarerat namn.
        /// </summary>
        RelationshipKind = 0,

        /// <summary>
        /// Sorterar på deklarerat namn.
        /// </summary>
        DeclaredName = 1,

        /// <summary>
        /// Sorterar på mapping-status.
        /// </summary>
        MappingStatus = 2,

        /// <summary>
        /// Sorterar på verification-status.
        /// </summary>
        VerificationStatus = 3,

        /// <summary>
        /// Sorterar på första observation.
        /// </summary>
        FirstObservedUtc = 4,

        /// <summary>
        /// Sorterar på senaste observation.
        /// </summary>
        LastObservedUtc = 5,

        /// <summary>
        /// Sorterar på senaste verifiering.
        /// </summary>
        LastVerifiedUtc = 6
    }
}
