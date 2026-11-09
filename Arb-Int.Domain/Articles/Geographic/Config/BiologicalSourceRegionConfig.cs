using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class BiologicalSourceRegionConfig :
        IEntityTypeConfiguration<BiologicalSourceRegion>
    {
        public void Configure(EntityTypeBuilder<BiologicalSourceRegion> builder)
        {
            builder.ToTable("BiologicalSourceRegions");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.BiologicalSourceId,
                x.GeographicLocationId
            })
            .IsUnique();

            builder.HasIndex(x => x.GeographicLocationId);

            builder.HasOne(x => x.BiologicalSource)
                .WithMany(x => x.RegionLinks)
                .HasForeignKey(x => x.BiologicalSourceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.GeographicLocation)
                .WithMany(x => x.BiologicalSourceLinks)
                .HasForeignKey(x => x.GeographicLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}