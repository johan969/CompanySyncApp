using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class ConstituentTypeConfig :
        IEntityTypeConfiguration<ConstituentType>
    {
        public void Configure(EntityTypeBuilder<ConstituentType> builder)
        {
            builder.ToTable("ConstituentTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Code)
                .IsUnique();
        }
    }
}