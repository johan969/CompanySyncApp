using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class MaterialCharacteristic : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid MaterialId { get; set; }
        public Material Material { get; set; } = null!;

        public Guid CharacteristicId { get; set; }
        public Characteristic Characteristic { get; set; } = null!;
    }
}