namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller en typ av affärsrelation.
    /// </summary>
    public sealed class BusinessRelationshipTypeResponse
    {
        /// <summary>
        /// Identifierar relationstypen.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Relationstypens stabila maskinkod.
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// Relationstypens visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Anger om relationstypen kan användas för nya relationer.
        /// </summary>
        public bool IsActive { get; init; }
    }
}