namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller organisationsspecifik information för en Party.
    /// </summary>
    public sealed class OrganizationPartyResponse
    {
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
        /// Anger om organisationen tillhör den interna organisationen.
        /// </summary>
        public bool? IsInternal { get; init; }

        /// <summary>
        /// Land där organisationen är registrerad.
        /// </summary>
        public Guid? RegistrationCountryId { get; init; }

        /// <summary>
        /// Identifierare för eventuell moderorganisation.
        /// </summary>
        public Guid? ParentOrganizationId { get; init; }

        /// <summary>
        /// Lättviktsreferens till eventuell moderorganisation.
        /// </summary>
        public PartyReferenceResponse? ParentOrganization { get; init; }

        /// <summary>
        /// Antal direkta underorganisationer.
        /// </summary>
        public int ChildOrganizationCount { get; init; }

        /// <summary>
        /// Antal organisationsidentifierare.
        /// </summary>
        public int IdentifierCount { get; init; }

        /// <summary>
        /// Antal personrelationer till organisationen.
        /// </summary>
        public int PersonLinkCount { get; init; }
    }
}