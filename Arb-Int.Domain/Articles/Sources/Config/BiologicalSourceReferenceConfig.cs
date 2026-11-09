using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class BiologicalSourceReferenceConfig :
        IEntityTypeConfiguration<BiologicalSourceReference>
    {
        public void Configure(
            EntityTypeBuilder<BiologicalSourceReference> builder)
        {
            builder.ToTable("BiologicalSourceReferences");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.BiologicalSourceId,
                x.ReferenceEntryId
            })
            .IsUnique();

            builder.HasIndex(x => new
            {
                x.BiologicalSourceId,
                x.SortOrder
            });

            builder.HasOne(x => x.BiologicalSource)
                .WithMany(x => x.ReferenceLinks)
                .HasForeignKey(x => x.BiologicalSourceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ReferenceEntry)
                .WithMany(x => x.BiologicalSourceLinks)
                .HasForeignKey(x => x.ReferenceEntryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}