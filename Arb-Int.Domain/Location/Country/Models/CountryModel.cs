using Arb_Int.Domain.Base;
using Arb_Int.Domain.Economy;
using Arb_Int.Domain.Parties;


namespace Arb_Int.Domain.Location
{
    public class Country : IEntity<Guid>, INamed
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsEUCountry { get; set; } = false;
        public string? DefaultTimeZone { get; set; }
        public string? Alpha2Code { get; set; }
        /// <summary>
        /// ISO-3166-3
        /// </summary>
        public string? Alpha3Code { get; set; }
        /// <summary>
        /// ISO-3166 Numeric
        /// </summary>
        public string? ISO3166Numeric { get; set; }
        public double? DefaultVAT { get; set; }
        public int? CurrencyId { get; set; }
        public Currency? Currency { get; set; }
        public required int ContinentId { get; set; }
        public Continent Continent { get; set; } = null!;
        public ICollection<Address> Addresses { get; set; } = [];
        public ICollection<CountryWater> CountryWaters { get; set; } = [];
    }
}