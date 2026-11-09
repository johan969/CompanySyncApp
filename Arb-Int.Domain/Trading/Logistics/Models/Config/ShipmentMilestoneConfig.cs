using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ShipmentMilestoneConfig : IEntityTypeConfiguration<ShipmentMilestone>
    {
        public void Configure(EntityTypeBuilder<ShipmentMilestone> builder)
        {
            builder.ToTable("ShipmentMilestones");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.ExternalReference)
                .HasMaxLength(255);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.ShipmentId,
                x.ShipmentMilestoneTypeId,
                x.PlannedAtUtc
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

            builder.HasOne(x => x.ShipmentMilestoneType)
                .WithMany()
                .HasForeignKey(x => x.ShipmentMilestoneTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
