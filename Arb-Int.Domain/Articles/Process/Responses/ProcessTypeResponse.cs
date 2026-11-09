namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Innehåller reference-data för en process type.
    /// </summary>
    public sealed class ProcessTypeResponse
    {
        /// <summary>
        /// Identifierar process typen.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Anger process typens stabila kod.
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// Anger process typens visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Beskriver process typen.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// Anger den kurerade visningsordningen.
        /// </summary>
        public int SortOrder { get; init; }

        /// <summary>
        /// Anger om reference-posten är aktiv.
        /// </summary>
        public bool IsActive { get; init; }
    }
}
