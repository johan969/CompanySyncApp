using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Parties.Configurations
{
    public class PersonConfig : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("People");

            builder.Property(x => x.GivenName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.MiddleName)
                .HasMaxLength(100);

            builder.Property(x => x.FamilyName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.PreferredName)
                .HasMaxLength(100);

            builder.Property(x => x.Honorific)
                .HasMaxLength(50);

            builder.Property(x => x.PreferredLanguageCode)
                .HasMaxLength(16);

            builder.HasIndex(x => new
            {
                x.FamilyName,
                x.GivenName
            });
        }
    }
}