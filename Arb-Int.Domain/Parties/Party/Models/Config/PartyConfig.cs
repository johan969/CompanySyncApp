using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Parties.Configurations
{
    public class PartyConfig : IEntityTypeConfiguration<Party>
    {
        public void Configure(EntityTypeBuilder<Party> builder)
        {
            builder.UseTptMappingStrategy();

            builder.ToTable("Parties");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NormalizedName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(x => x.NormalizedName);

            builder.HasIndex(x => x.IsActive);
        }
    }
}