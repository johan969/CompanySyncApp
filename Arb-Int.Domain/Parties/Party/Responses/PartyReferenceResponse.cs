namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller en lättviktsreferens till en Party.
    /// </summary>
    public sealed class PartyReferenceResponse
    {
        /// <summary>
        /// Partens identifierare.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Partens primära visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Anger om parten är en organisation eller person.
        /// </summary>
        public PartyType PartyType { get; init; }

        /// <summary>
        /// Anger om parten är aktiv.
        /// </summary>
        public bool IsActive { get; init; }
    }
}