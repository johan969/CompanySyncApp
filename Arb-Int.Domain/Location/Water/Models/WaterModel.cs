using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Location
{
    public enum WaterType
    {
        Freshwater,
        Saltwater,
        Brackish
    }
    public enum WaterFeatureType
    {
        Ocean,
        Sea,
        InlandSea,
        SeaRegion,
        MarineRegion,
        Gulf,
        Bay,
        Bight,
        Strait,
        Channel,
        Lagoon,
        Lake
    }
    public class Water : IEntity<Guid>, INamed, IDescribable
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public WaterType Type { get; set; }
        public WaterFeatureType FeatureType { get; set; }

        public string Description { get; set; } = string.Empty;

        public Guid? ParentWaterId { get; set; }

        public Water? ParentWater { get; set; }

        public ICollection<Water> ChildWaters { get; set; } = new List<Water>();

        public ICollection<CountryWater> CountryWaters { get; set; } = [];
    }

}
