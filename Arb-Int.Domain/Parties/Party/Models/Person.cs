namespace Arb_Int.Domain.Parties
{
    public class Person : Party
    {
        public string GivenName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string FamilyName { get; set; } = null!;

        public string? PreferredName { get; set; }

        /// <summary>
        /// Exempelvis Dr, Prof eller annan titel.
        /// Befattning på ett företag ligger i OrganizationPerson.
        /// </summary>
        public string? Honorific { get; set; }

        /// <summary>
        /// BCP-47-kod, exempelvis sv, en eller sv-SE.
        /// </summary>
        public string? PreferredLanguageCode { get; set; }

        public ICollection<OrganizationPerson> OrganizationLinks { get; set; } = [];

        public PersonUserAccount? UserAccount { get; set; }
    }
}