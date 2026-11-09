using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class BiologicalSourceCharacteristic : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid BiologicalSourceId { get; set; }
        public BiologicalSource BiologicalSource { get; set; } = null!;

        public Guid CharacteristicId { get; set; }
        public Characteristic Characteristic { get; set; } = null!;
    }
}