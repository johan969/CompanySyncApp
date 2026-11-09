namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller data som krävs för att skapa en organisation.
    /// </summary>
    public sealed class CreateOrganizationRequest
    {
        /// <summary>
        /// Organisationens primära visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Organisationens juridiska namn.
        /// </summary>
        public required string LegalName { get; init; }

        /// <summary>
        /// Organisationens handels- eller varumärkesnamn.
        /// </summary>
        public string? TradingName { get; init; }

        /// <summary>
        /// Organisationens typ.
        /// </summary>
        public OrganizationKind OrganizationKind { get; init; }

        /// <summary>
        /// Anger om organisationen är intern.
        /// </summary>
        public bool? IsInternal { get; init; }

        /// <summary>
        /// Land där organisationen är registrerad.
        /// </summary>
        public Guid? RegistrationCountryId { get; init; }

        /// <summary>
        /// Eventuell moderorganisation.
        /// </summary>
        public Guid? ParentOrganizationId { get; init; }

        /// <summary>
        /// Organisationens beskrivning.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Interna anteckningar om organisationen.
        /// </summary>
        public string? Notes { get; init; }
    }
}