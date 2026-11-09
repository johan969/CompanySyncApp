namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Innehåller reference-data för ett material.
    /// </summary>
    public sealed class MaterialResponse
    {
        /// <summary>
        /// Identifierar materialet.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Anger materialets visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Anger materialets stabila slug.
        /// </summary>
        public required string Slug { get; init; }

        /// <summary>
        /// Beskriver materialet.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// Identifierar materialets normaliserade materialtyp.
        /// </summary>
        public int MaterialTypeId { get; init; }

        /// <summary>
        /// Anger materialtypens visningsnamn.
        /// </summary>
        public required string MaterialTypeName { get; init; }

        /// <summary>
        /// Anger om materialtypen är en rot i förädlingskedjan.
        /// </summary>
        public bool IsRawMaterial { get; init; }

        /// <summary>
        /// Anger om reference-posten är aktiv.
        /// </summary>
        public bool IsActive { get; init; }
    }
}
