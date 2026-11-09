using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// MaterialOrigin = det deklarerade ursprunget för en faktisk råvara eller materialvariant
    /// </summary>
    public class MaterialOrigin : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid MaterialId { get; set; }
        public Material Material { get; set; } = null!;

        public Guid GeographicLocationId { get; set; }
        public GeographicLocation GeographicLocation { get; set; } = null!;

        public MaterialOriginRole Role { get; set; }

        public string? Notes { get; set; }
    }
}