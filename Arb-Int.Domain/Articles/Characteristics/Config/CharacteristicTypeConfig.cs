using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class CharacteristicTypeConfig :
        IEntityTypeConfiguration<CharacteristicType>
    {
        public void Configure(EntityTypeBuilder<CharacteristicType> builder)
        {
            builder.ToTable("CharacteristicTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Code)
                .IsUnique();
        }
    }
}