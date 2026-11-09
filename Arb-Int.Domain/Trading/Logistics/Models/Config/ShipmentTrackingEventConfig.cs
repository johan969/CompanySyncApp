using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ShipmentTrackingEventConfig : IEntityTypeConfiguration<ShipmentTrackingEvent>
    {
        public void Configure(EntityTypeBuilder<ShipmentTrackingEvent> builder)
        {
            builder.ToTable("ShipmentTrackingEvents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.EventCode)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.EventName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.LocationText)
                .HasMaxLength(1000);

            builder.Property(x => x.ExternalEventId)
                .HasMaxLength(255);

            builder.Property(x => x.PayloadJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.ShipmentId,
                x.OccurredAtUtc
            });

            builder.HasIndex(x => new
            {
                x.ExternalSystemId,
                x.ExternalEventId
            });

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.ShipmentLegId,
                    x.ShipmentId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.ShipmentId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ExternalSystem)
                .WithMany()
                .HasForeignKey(x => x.ExternalSystemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Location)
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
