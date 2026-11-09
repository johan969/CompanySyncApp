namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller redigerbar metadata för en aktiv kontaktpunkt.
    /// </summary>
    public sealed class UpdatePartyContactPointRequest
    {
        /// <summary>
        /// Frivillig etikett för kontaktpunkten.
        /// </summary>
        public string? Label { get; init; }

        /// <summary>
        /// Eventuell telefonanknytning.
        /// </summary>
        public string? Extension { get; init; }

        /// <summary>
        /// Anger om kontaktpunkten ska vara primär.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Anger om kontaktuppgiften är verifierad.
        /// </summary>
        public bool IsVerified { get; init; }

        /// <summary>
        /// Anger kontaktbehörigheten.
        /// </summary>
        public ContactPermissionStatus PermissionStatus { get; init; }

        /// <summary>
        /// Interna anteckningar.
        /// </summary>
        public string? Notes { get; init; }
    }
}