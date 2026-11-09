using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Sourcing
{
    public class SupplierMaterialOfferingOrigin : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid SupplierMaterialOfferingId { get; set; }

        public SupplierMaterialOffering SupplierMaterialOffering { get; set; } = null!;

        public Guid GeographicLocationId { get; set; }

        public GeographicLocation GeographicLocation { get; set; } = null!;

        public SupplierOfferingOriginRole Role { get; set; }

        public bool IsPrimary { get; set; }

        public string? Notes { get; set; }
    }
}