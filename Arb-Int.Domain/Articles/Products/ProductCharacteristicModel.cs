using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class ProductCharacteristic : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public Guid CharacteristicId { get; set; }
        public Characteristic Characteristic { get; set; } = null!;
    }
}