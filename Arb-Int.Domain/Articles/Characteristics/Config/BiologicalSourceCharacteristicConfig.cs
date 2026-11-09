using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class BiologicalSourceCharacteristicConfig :
        IEntityTypeConfiguration<BiologicalSourceCharacteristic>
    {
        public void Configure(
            EntityTypeBuilder<BiologicalSourceCharacteristic> builder)
        {
            builder.ToTable("BiologicalSourceCharacteristics");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.BiologicalSourceId,
                x.CharacteristicId
            })
            .IsUnique();

            builder.HasIndex(x => x.CharacteristicId);

            builder.HasOne(x => x.BiologicalSource)
                .WithMany(x => x.CharacteristicLinks)
                .HasForeignKey(x => x.BiologicalSourceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Characteristic)
                .WithMany(x => x.BiologicalSourceLinks)
                .HasForeignKey(x => x.CharacteristicId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}