using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ShipmentLegConfig : IEntityTypeConfiguration<ShipmentLeg>
    {
        public void Configure(EntityTypeBuilder<ShipmentLeg> builder)
        {
            builder.ToTable("ShipmentLegs");

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.ShipmentId
            });

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Mode)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.TrackingNumber)
                .HasMaxLength(255);

            builder.Property(x => x.BookingReference)
                .HasMaxLength(255);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.ShipmentId,
                x.SequenceNumber
            })
                .IsUnique();

            builder.HasOne(x => x.CarrierBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.CarrierBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FromLocation)
                .WithMany()
                .HasForeignKey(x => x.FromLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ToLocation)
                .WithMany()
                .HasForeignKey(x => x.ToLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FromAddress)
                .WithMany()
                .HasForeignKey(x => x.FromAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ToAddress)
                .WithMany()
                .HasForeignKey(x => x.ToAddressId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
