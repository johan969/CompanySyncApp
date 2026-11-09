using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class ProductReferenceConfig :
        IEntityTypeConfiguration<ProductReference>
    {
        public void Configure(EntityTypeBuilder<ProductReference> builder)
        {
            builder.ToTable("ProductReferences");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.ProductId,
                x.ReferenceEntryId
            })
            .IsUnique();

            builder.HasIndex(x => new
            {
                x.ProductId,
                x.SortOrder
            });

            builder.HasIndex(x => x.ReferenceEntryId);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.ReferenceLinks)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ReferenceEntry)
                .WithMany(x => x.ProductLinks)
                .HasForeignKey(x => x.ReferenceEntryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}