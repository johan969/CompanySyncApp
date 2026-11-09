using Arb_Int.Domain.Base;
using Arb_Int.Domain.Location;

namespace Arb_Int.Domain.Parties
{
    public class Organization : Party, INormalizedLegalName
    {
        /// <summary>
        /// Organisationens juridiska namn.
        /// </summary>
        public required string LegalName { get; set; }

        public string NormalizedLegalName { get; set; } = null!;

        /// <summary>
        /// Handelsnamn eller varumärkesnamn om det skiljer sig
        /// från det juridiska namnet.
        /// </summary>
        public string? TradingName { get; set; }

        public OrganizationKind OrganizationKind { get; set; }
        public bool? IsInternal { get; set; }

        public Guid? RegistrationCountryId { get; set; }
        public Country? RegistrationCountry { get; set; }

        /// <summary>
        /// Används för koncern-, dotterbolags- och filialstrukturer.
        /// </summary>
        public Guid? ParentOrganizationId { get; set; }
        public Organization? ParentOrganization { get; set; }

        public ICollection<Organization> ChildOrganizations { get; set; } = [];

        public ICollection<OrganizationIdentifier> Identifiers { get; set; } = [];

        public ICollection<OrganizationPerson> PersonLinks { get; set; } = [];
    }
}