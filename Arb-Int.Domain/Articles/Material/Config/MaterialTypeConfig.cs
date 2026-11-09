using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class MaterialTypeConfig :
        IEntityTypeConfiguration<MaterialType>
    {
        public void Configure(EntityTypeBuilder<MaterialType> builder)
        {
            builder.ToTable("MaterialTypes", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_MaterialTypes_Parent_NotSelf",
                    "\"ParentMaterialTypeId\" IS NULL " +
                    "OR \"ParentMaterialTypeId\" <> \"Id\"");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasOne(x => x.ParentMaterialType)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentMaterialTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}