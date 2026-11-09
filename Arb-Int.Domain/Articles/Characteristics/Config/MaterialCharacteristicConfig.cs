using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class MaterialCharacteristicConfig :
        IEntityTypeConfiguration<MaterialCharacteristic>
    {
        public void Configure(EntityTypeBuilder<MaterialCharacteristic> builder)
        {
            builder.ToTable("MaterialCharacteristics");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.MaterialId,
                x.CharacteristicId
            })
            .IsUnique();

            builder.HasIndex(x => x.CharacteristicId);

            builder.HasOne(x => x.Material)
                .WithMany(x => x.CharacteristicLinks)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Characteristic)
                .WithMany(x => x.MaterialLinks)
                .HasForeignKey(x => x.CharacteristicId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}