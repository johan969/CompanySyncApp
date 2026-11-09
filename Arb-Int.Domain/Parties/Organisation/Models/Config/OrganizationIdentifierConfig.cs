using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Parties.Configurations
{
    public class OrganizationIdentifierConfig :
        IEntityTypeConfiguration<OrganizationIdentifier>
    {
        public void Configure(
            EntityTypeBuilder<OrganizationIdentifier> builder)
        {
            builder.ToTable("OrganizationIdentifiers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Scheme)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.NormalizedValue)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Issuer)
                .HasMaxLength(255);

            builder.Property(x => x.IdentifierKey)
                .IsRequired()
                .HasMaxLength(128);

            builder.HasIndex(x => x.IdentifierKey)
                .IsUnique();

            builder.HasIndex(x => x.OrganizationId);

            builder.HasIndex(x => new
            {
                x.OrganizationId,
                x.Scheme
            })
            .IsUnique()
            .HasFilter("\"IsPrimary\" = TRUE");

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.Identifiers)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.IssuingCountry)
                .WithMany()
                .HasForeignKey(x => x.IssuingCountryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}