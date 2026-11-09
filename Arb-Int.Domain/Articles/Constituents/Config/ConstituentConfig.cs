using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class ConstituentConfig :
        IEntityTypeConfiguration<Constituent>
    {
        public void Configure(EntityTypeBuilder<Constituent> builder)
        {
            builder.ToTable("Constituents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NormalizedName)
                .IsRequired()
                .HasMaxLength(512);

            builder.HasIndex(x => x.NormalizedName)
                .IsUnique();

            builder.HasOne(x => x.ConstituentType)
                .WithMany(x => x.Constituents)
                .HasForeignKey(x => x.ConstituentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}