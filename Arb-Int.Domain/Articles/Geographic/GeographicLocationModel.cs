using Arb_Int.Domain.Base;
using Arb_Int.Domain.Location;

namespace Arb_Int.Domain.Articles
{
    public class GeographicLocation : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public GeographicScope Scope { get; set; }

        public Guid? CountryId { get; set; }
        public Country? Country { get; set; }

        public Guid? WaterId { get; set; }
        public Water? Water { get; set; }

        /// <summary>
        /// Fri text. Kan vara administrativ region, odlingsområde,
        /// produktionssystem eller annan lokal avgränsning.
        /// </summary>
        public string? Subregion { get; set; }

        /// <summary>
        /// Deterministisk normaliserad nyckel.
        /// <para>Exempel:<br />
        /// country:sweden|water:skagerrak|subregion:<br />
        /// country:usa|water:pacific-ocean|subregion:california<br />
        /// global</para>
        /// </summary>
        public string LocationKey { get; set; } = null!;

        public ICollection<BiologicalSourceRegion> BiologicalSourceLinks { get; set; } = [];

        public ICollection<MaterialOrigin> MaterialOriginLinks { get; set; } = [];
    }
}