using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class Characteristic : IEntity<Guid>, INamed, INormalizedName
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string NormalizedName { get; set; } = null!;

        public int CharacteristicTypeId { get; set; }
        public CharacteristicType CharacteristicType { get; set; } = null!;

        public ICollection<BiologicalSourceCharacteristic> BiologicalSourceLinks { get; set; } = [];

        public ICollection<MaterialCharacteristic> MaterialLinks { get; set; } = [];
        public ICollection<ProductCharacteristic> ProductLinks { get; set; } = [];
    }
}