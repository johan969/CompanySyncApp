using Arb_Int.Domain.Base;
using Arb_Int.Domain.Location;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// En normaliserad adress.
    /// <para>Adressraden bör behandlas som ett värdeobjekt i applikationslogiken.
    /// Om adressen ändras är det ofta bättre att skapa en ny adressrad och
    /// avsluta den gamla PartyAddress-kopplingen än att skriva över historik.</para>
    /// </summary>
    public class Address : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public string? CareOf { get; set; }

        public string? PostOfficeBox { get; set; }

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string? AddressLine3 { get; set; }

        public string? PostalCode { get; set; }

        public string? Locality { get; set; }

        public string? AdministrativeArea { get; set; }

        public Guid CountryId { get; set; }
        public Country Country { get; set; } = null!;

        /// <summary>
        /// Deterministisk normaliserad adressnyckel.
        /// </summary>
        public string AddressKey { get; set; } = null!;

        public ICollection<PartyAddress> PartyLinks { get; set; } = [];
    }
}