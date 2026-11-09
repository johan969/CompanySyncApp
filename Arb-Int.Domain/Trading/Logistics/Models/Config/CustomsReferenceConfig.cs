using Arb_Int.Domain.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class CustomsReferenceConfig : IEntityTypeConfiguration<CustomsReference>
    {
        public void Configure(EntityTypeBuilder<CustomsReference> builder)
        {
            builder.ToTable("CustomsReferences");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Kind)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.ReferenceNumber)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.CommodityCode)
                .HasMaxLength(100);

            builder.Property(x => x.Status)
                .HasMaxLength(100);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.ShipmentId,
                x.Kind,
                x.ReferenceNumber
            })
                .IsUnique();

            builder.HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ShipmentDocument)
                .WithMany()
                .HasForeignKey(x => x.ShipmentDocumentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
