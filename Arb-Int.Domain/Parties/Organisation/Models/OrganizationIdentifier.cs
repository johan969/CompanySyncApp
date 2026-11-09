using Arb_Int.Domain.Base;
using Arb_Int.Domain.Location;

namespace Arb_Int.Domain.Parties
{
    public class OrganizationIdentifier : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;

        /// <summary>
        /// Maskinkod för identifierartypen.
        /// <para>Exempel:<br />
        /// organization-number<br />
        /// vat<br />
        /// lei<br />
        /// duns<br />
        /// eori</para>
        /// </summary>
        public string Scheme { get; set; } = null!;

        public string Value { get; set; } = null!;

        public string NormalizedValue { get; set; } = null!;

        public string? Issuer { get; set; }

        public Guid? IssuingCountryId { get; set; }
        public Country? IssuingCountry { get; set; }

        public bool IsPrimary { get; set; }

        /// <summary>
        /// Deterministisk nyckel, exempelvis SHA-256 av:<br />
        /// scheme + issuing-country + normalized-value
        /// </summary>
        public string IdentifierKey { get; set; } = null!;
    }
}