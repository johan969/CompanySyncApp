using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ShipmentConfig : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable("Shipments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.ShipmentNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Mode)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.MasterTrackingNumber)
                .HasMaxLength(255);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => x.ShipmentNumber)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.Status,
                x.PlannedPickupAtUtc,
                x.PlannedDeliveryAtUtc
            });

            builder.HasOne(x => x.FreightForwarderBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.FreightForwarderBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OriginLocation)
                .WithMany()
                .HasForeignKey(x => x.OriginLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DestinationLocation)
                .WithMany()
                .HasForeignKey(x => x.DestinationLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OriginAddress)
                .WithMany()
                .HasForeignKey(x => x.OriginAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DestinationAddress)
                .WithMany()
                .HasForeignKey(x => x.DestinationAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Legs)
                .WithOne(x => x.Shipment)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.OrderLineAllocations)
                .WithOne(x => x.Shipment)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.LotAllocations)
                .WithOne(x => x.Shipment)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Milestones)
                .WithOne(x => x.Shipment)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.TrackingEvents)
                .WithOne(x => x.Shipment)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CarrierBookings)
                .WithOne(x => x.Shipment)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Documents)
                .WithOne(x => x.Shipment)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CustomsReferences)
                .WithOne(x => x.Shipment)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
