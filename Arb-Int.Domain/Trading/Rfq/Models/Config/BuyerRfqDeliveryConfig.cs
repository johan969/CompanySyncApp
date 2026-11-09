using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqDeliveryConfig : IEntityTypeConfiguration<BuyerRfqDelivery>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqDelivery> builder)
        {
            builder.ToTable(
                "BuyerRfqDeliveries",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerRfqDeliveries_Window",
                        """
                    "RequestedDeliveryToUtc" >= "RequestedDeliveryFromUtc"
                    """);

                    table.HasCheckConstraint(
                        "CK_BuyerRfqDeliveries_Temperature",
                        """
                    "MinimumTemperatureCelsius" IS NULL
                    OR "MaximumTemperatureCelsius" IS NULL
                    OR "MaximumTemperatureCelsius" >= "MinimumTemperatureCelsius"
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerRfqRevisionId
            });

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.NamedPlace)
                .HasMaxLength(512);

            builder.Property(x => x.MinimumTemperatureCelsius)
                .HasPrecision(8, 3);

            builder.Property(x => x.MaximumTemperatureCelsius)
                .HasPrecision(8, 3);

            builder.Property(x => x.PackagingNotes)
                .HasMaxLength(4096);

            builder.Property(x => x.DeliveryNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.BuyerRfqRevisionId,
                x.DeliveryNumber
            })
                .IsUnique();

            builder.HasOne(x => x.DestinationAddress)
                .WithMany()
                .HasForeignKey(x => x.DestinationAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DestinationLocation)
                .WithMany()
                .HasForeignKey(x => x.DestinationLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.IncotermRule)
                .WithMany()
                .HasForeignKey(x => x.IncotermRuleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
