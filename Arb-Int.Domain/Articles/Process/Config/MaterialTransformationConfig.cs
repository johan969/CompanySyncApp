using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class MaterialTransformationConfig :
        IEntityTypeConfiguration<MaterialTransformation>
    {
        public void Configure(
            EntityTypeBuilder<MaterialTransformation> builder)
        {
            builder.ToTable("MaterialTransformations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.Property(x => x.Version)
                .HasMaxLength(64);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(x => x.ProcessTypeId);

            builder.HasOne(x => x.ProcessType)
                .WithMany(x => x.Transformations)
                .HasForeignKey(x => x.ProcessTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}