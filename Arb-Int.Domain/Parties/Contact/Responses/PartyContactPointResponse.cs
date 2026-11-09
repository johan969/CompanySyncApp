namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller en kontaktpunkt för en organisation eller person.
    /// </summary>
    public sealed class PartyContactPointResponse
    {
        /// <summary>
        /// Kontaktpunktens identifierare.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Parten som kontaktpunkten tillhör.
        /// </summary>
        public Guid PartyId { get; init; }

        /// <summary>
        /// Eventuell organisationskontext för en persons kontaktuppgift.
        /// </summary>
        public Guid? OrganizationPersonId { get; init; }

        /// <summary>
        /// Organisationen som den kontextuella kontaktuppgiften gäller.
        /// </summary>
        public Guid? OrganizationId { get; init; }

        /// <summary>
        /// Kontaktpunktens typ.
        /// </summary>
        public ContactPointKind Kind { get; init; }

        /// <summary>
        /// Frivillig etikett, exempelvis Växel eller Arbetsmail.
        /// </summary>
        public string? Label { get; init; }

        /// <summary>
        /// Kontaktvärdet.
        /// </summary>
        public required string Value { get; init; }

        /// <summary>
        /// Eventuell anknytning eller extension.
        /// </summary>
        public string? Extension { get; init; }

        /// <summary>
        /// Anger om kontaktpunkten är primär inom sin typ och kontext.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Anger om kontaktuppgiften är verifierad.
        /// </summary>
        public bool IsVerified { get; init; }

        /// <summary>
        /// Anger vilken kontaktbehörighet som gäller.
        /// </summary>
        public ContactPermissionStatus PermissionStatus { get; init; }

        /// <summary>
        /// Tidpunkt då kontaktpunkten började gälla.
        /// </summary>
        public DateTimeOffset ValidFromUtc { get; init; }

        /// <summary>
        /// Tidpunkt då kontaktpunkten slutade gälla.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Interna anteckningar.
        /// </summary>
        public string? Notes { get; init; }
    }
}