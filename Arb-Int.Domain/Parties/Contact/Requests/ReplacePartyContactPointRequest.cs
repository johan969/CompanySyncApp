namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller data för att ersätta en aktiv kontaktpunkt.
    /// </summary>
    public sealed class ReplacePartyContactPointRequest
    {
        /// <summary>
        /// Eventuell ny OrganizationPerson-kontext.
        /// </summary>
        public Guid? OrganizationPersonId { get; init; }

        /// <summary>
        /// Kontaktpunktens nya typ.
        /// </summary>
        public ContactPointKind Kind { get; init; }

        /// <summary>
        /// Frivillig etikett.
        /// </summary>
        public string? Label { get; init; }

        /// <summary>
        /// Det nya kontaktvärdet.
        /// </summary>
        public required string Value { get; init; }

        /// <summary>
        /// Eventuell telefonanknytning.
        /// </summary>
        public string? Extension { get; init; }

        /// <summary>
        /// Anger om den nya kontaktpunkten ska vara primär.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Anger om den nya kontaktuppgiften är verifierad.
        /// </summary>
        public bool IsVerified { get; init; }

        /// <summary>
        /// Anger kontaktbehörigheten.
        /// </summary>
        public ContactPermissionStatus PermissionStatus { get; init; }

        /// <summary>
        /// Tidpunkt då den nya kontaktpunkten börjar gälla. Aktuell UTC-tid används om värdet saknas.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Interna anteckningar.
        /// </summary>
        public string? Notes { get; init; }
    }
}