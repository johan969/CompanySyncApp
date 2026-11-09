namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Innehåller reference-data för en material type.
    /// </summary>
    public sealed class MaterialTypeResponse
    {
        /// <summary>
        /// Identifierar material typen.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Anger material typens stabila kod.
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// Anger material typens visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Beskriver material typen.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// Anger den kurerade visningsordningen.
        /// </summary>
        public int SortOrder { get; init; }

        /// <summary>
        /// Anger om typen är en rot i förädlingskedjan.
        /// </summary>
        public bool IsRawMaterial { get; init; }

        /// <summary>
        /// Identifierar överordnad material type.
        /// </summary>
        public int? ParentMaterialTypeId { get; init; }

        /// <summary>
        /// Anger överordnad material types visningsnamn.
        /// </summary>
        public string? ParentMaterialTypeName { get; init; }

        /// <summary>
        /// Anger om reference-posten är aktiv.
        /// </summary>
        public bool IsActive { get; init; }
    }
}
