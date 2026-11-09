using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class CharacteristicConfig :
        IEntityTypeConfiguration<Characteristic>
    {
        public void Configure(EntityTypeBuilder<Characteristic> builder)
        {
            builder.ToTable("Characteristics");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NormalizedName)
                .IsRequired()
                .HasMaxLength(512);

            builder.HasIndex(x => new
            {
                x.CharacteristicTypeId,
                x.NormalizedName
            })
            .IsUnique();

            builder.HasOne(x => x.CharacteristicType)
                .WithMany(x => x.Characteristics)
                .HasForeignKey(x => x.CharacteristicTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}