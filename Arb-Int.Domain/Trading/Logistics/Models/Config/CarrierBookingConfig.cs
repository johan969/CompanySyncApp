using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class CarrierBookingConfig : IEntityTypeConfiguration<CarrierBooking>
    {
        public void Configure(EntityTypeBuilder<CarrierBooking> builder)
        {
            builder.ToTable("CarrierBookings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.BookingReference)
                .HasMaxLength(255);

            builder.Property(x => x.ExternalBookingId)
                .HasMaxLength(255);

            builder.Property(x => x.CostAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.ServiceLevel)
                .HasMaxLength(255);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.ShipmentId,
                x.CarrierBusinessRelationshipId,
                x.Status
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

            builder.HasOne(x => x.CarrierBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.CarrierBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ExternalSystem)
                .WithMany()
                .HasForeignKey(x => x.ExternalSystemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CostCurrency)
                .WithMany()
                .HasForeignKey(x => x.CostCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
