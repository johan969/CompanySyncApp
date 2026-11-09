using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerSalesOrderDeliveryConfig : IEntityTypeConfiguration<BuyerSalesOrderDelivery>
    {
        public void Configure(EntityTypeBuilder<BuyerSalesOrderDelivery> builder)
        {
            builder.ToTable(
                "BuyerSalesOrderDeliveries",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerSalesOrderDeliveries_Window",
                        """
                    "PlannedDeliveryToUtc" >= "PlannedDeliveryFromUtc"
                    """);
                });
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerSalesOrderId
            });
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.NamedPlace)
                .HasMaxLength(512);
            builder.Property(x => x.DeliveryInstructions)
                .HasMaxLength(4096);
            builder.HasIndex(x => new
            {
                x.BuyerSalesOrderId,
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
            builder.HasMany(x => x.Lines)
                .WithOne(x => x.BuyerSalesOrderDelivery)
                .HasForeignKey(x => new
                {
                    x.BuyerSalesOrderDeliveryId,
                    x.BuyerSalesOrderId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerSalesOrderId
                })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
