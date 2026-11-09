using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class ProductCharacteristicConfig :
        IEntityTypeConfiguration<ProductCharacteristic>
    {
        public void Configure(EntityTypeBuilder<ProductCharacteristic> builder)
        {
            builder.ToTable("ProductCharacteristics");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.ProductId,
                x.CharacteristicId
            })
            .IsUnique();

            builder.HasIndex(x => x.CharacteristicId);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.CharacteristicLinks)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Characteristic)
                .WithMany(x => x.ProductLinks)
                .HasForeignKey(x => x.CharacteristicId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}