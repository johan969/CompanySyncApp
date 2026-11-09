namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller ett steg i livscykeln för en viss affärsrelationstyp.
    /// </summary>
    public sealed class BusinessRelationshipStageResponse
    {
        /// <summary>
        /// Identifierar relationens steg.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Identifierar relationstypen som steget tillhör.
        /// </summary>
        public int BusinessRelationshipTypeId { get; init; }

        /// <summary>
        /// Stegets stabila maskinkod.
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// Stegets visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Anger stegets sorteringsordning.
        /// </summary>
        public int SortOrder { get; init; }

        /// <summary>
        /// Anger om steget representerar en stängd relation.
        /// </summary>
        public bool IsClosed { get; init; }

        /// <summary>
        /// Anger om det stängda steget representerar ett lyckat utfall.
        /// </summary>
        public bool IsSuccessful { get; init; }
    }
}