namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller data för att skapa eller upserta en extern Party-referens.
    /// </summary>
    public sealed class CreatePartyExternalReferenceRequest
    {
        /// <summary>
        /// Källsystemets stabila namn, exempelvis legacy-mariadb eller supplier-directory.
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
        public DateTimeOffset? FirstSeenUtc { get; init; }

        /// <summary>
        /// Tidpunkt då posten senast observerades.
        /// </summary>
        public DateTimeOffset? LastSeenUtc { get; init; }

        /// <summary>
        /// Eventuell hash av det observerade innehållet.
        /// </summary>
        public string? ContentHash { get; init; }
    }
}