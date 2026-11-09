using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class MaterialReferenceConfig :
        IEntityTypeConfiguration<MaterialReference>
    {
        public void Configure(EntityTypeBuilder<MaterialReference> builder)
        {
            builder.ToTable("MaterialReferences");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.MaterialId,
                x.ReferenceEntryId
            })
            .IsUnique();

            builder.HasIndex(x => new
            {
                x.MaterialId,
                x.SortOrder
            });

            builder.HasOne(x => x.Material)
                .WithMany(x => x.ReferenceLinks)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ReferenceEntry)
                .WithMany(x => x.MaterialLinks)
                .HasForeignKey(x => x.ReferenceEntryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}