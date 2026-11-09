namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller en extern referens som kopplar en Party till ett källsystem.
    /// </summary>
    public sealed class PartyExternalReferenceResponse
    {
        /// <summary>
        /// Identifierar den externa referensen.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar parten som referensen tillhör.
        /// </summary>
        public Guid PartyId { get; init; }

        /// <summary>
        /// Källsystemets stabila namn.
        /// </summary>
        public required string SourceSystem { get; init; }

        /// <summary>
        /// Postens stabila identifierare inom källsystemet.
        /// </summary>
        public required string SourceRecordKey { get; init; }

        /// <summary>
        /// Eventuell URL till posten i källsystemet.
        /// </summary>
        public string? SourceUrl { get; init; }

        /// <summary>
        /// Tidpunkt då posten först observerades.
        /// </summary>
        public DateTimeOffset FirstSeenUtc { get; init; }

        /// <summary>
        /// Tidpunkt då posten senast observerades.
        /// </summary>
        public DateTimeOffset LastSeenUtc { get; init; }

        /// <summary>
        /// Eventuell hash av senast observerat innehåll.
        /// </summary>
        public string? ContentHash { get; init; }
    }
}