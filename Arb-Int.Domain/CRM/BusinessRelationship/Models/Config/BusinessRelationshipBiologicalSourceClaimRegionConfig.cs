using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class
        BusinessRelationshipBiologicalSourceClaimRegionConfig : IEntityTypeConfiguration<BusinessRelationshipBiologicalSourceClaimRegion>
    {
        public void Configure(EntityTypeBuilder<BusinessRelationshipBiologicalSourceClaimRegion> builder)
        {
            builder.ToTable(
                "BusinessRelationshipBiologicalSourceClaimRegions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(
                    BiologicalSourceClaimLocationRole.Unknown);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.BusinessRelationshipBiologicalSourceClaimId,
                x.GeographicLocationId,
                x.Role
            })
            .IsUnique();

            /*
             * Högst en primär region per claim.
             */
            builder.HasIndex(
                    x => x.BusinessRelationshipBiologicalSourceClaimId)
                .IsUnique()
                .HasFilter("\"IsPrimary\" = TRUE");

            builder.HasIndex(x => x.GeographicLocationId);

            builder.HasOne(x => x.Claim)
                .WithMany(x => x.RegionLinks)
                .HasForeignKey(
                    x => x.BusinessRelationshipBiologicalSourceClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.GeographicLocation)
                .WithMany()
                .HasForeignKey(x => x.GeographicLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}