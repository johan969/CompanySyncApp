using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class SupplierMaterialOfferingCharacteristicConfig : IEntityTypeConfiguration<SupplierMaterialOfferingCharacteristic>
    {
        public void Configure(EntityTypeBuilder<SupplierMaterialOfferingCharacteristic> builder)
        {
            builder.ToTable(
                "SupplierMaterialOfferingCharacteristics");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.SupplierMaterialOfferingId,
                x.CharacteristicId
            })
            .IsUnique();

            builder.HasIndex(x => x.CharacteristicId);

            builder.HasOne(x => x.SupplierMaterialOffering)
                .WithMany(x => x.CharacteristicLinks)
                .HasForeignKey(x => x.SupplierMaterialOfferingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Characteristic)
                .WithMany()
                .HasForeignKey(x => x.CharacteristicId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}