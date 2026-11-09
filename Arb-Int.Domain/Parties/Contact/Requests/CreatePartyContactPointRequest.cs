namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller data för att skapa en kontaktpunkt.
    /// </summary>
    public sealed class CreatePartyContactPointRequest
    {
        /// <summary>
        /// Eventuell OrganizationPerson-kontext för personens arbetskontakt.
        /// </summary>
        public Guid? OrganizationPersonId { get; init; }

        /// <summary>
        /// Typ av kontaktpunkt.
        /// </summary>
        public ContactPointKind Kind { get; init; }

        /// <summary>
        /// Frivillig etikett för kontaktpunkten.
        /// </summary>
        public string? Label { get; init; }

        /// <summary>
        /// Kontaktvärdet, exempelvis e-postadress eller telefonnummer.
        /// </summary>
        public required string Value { get; init; }

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
        /// Tidpunkt då kontaktpunkten börjar gälla. Aktuell UTC-tid används om värdet saknas.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Interna anteckningar.
        /// </summary>
        public string? Notes { get; init; }
    }
}