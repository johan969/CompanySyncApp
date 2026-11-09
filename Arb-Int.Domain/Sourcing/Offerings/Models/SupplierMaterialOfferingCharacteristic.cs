using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Sourcing
{
    public class SupplierMaterialOfferingCharacteristic : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid SupplierMaterialOfferingId { get; set; }

        public SupplierMaterialOffering SupplierMaterialOffering { get; set; } = null!;

        public Guid CharacteristicId { get; set; }

        public Characteristic Characteristic { get; set; } = null!;
    }
}