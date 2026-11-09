using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    /// <summary>
    /// Konfigurerar jurisdictions som hör till certification scheme-versioner.
    /// </summary>
    public sealed class CertificationSchemeJurisdictionConfig : IEntityTypeConfiguration<CertificationSchemeJurisdiction>
    {
        /// <summary>
        /// Konfigurerar nycklar, index, concurrency och relationer för jurisdictions.
        /// </summary>
        public void Configure(EntityTypeBuilder<CertificationSchemeJurisdiction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.CertificationSchemeId
            });

            builder.Property(x => x.Kind)
                .HasConversion<string>();

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasIndex(x => new
            {
                x.CertificationSchemeId,
                x.Code
            })
            .IsUnique();

            builder.HasIndex(x => x.CountryId);

            builder.HasOne(x => x.CertificationScheme)
                .WithMany(x => x.Jurisdictions)
                .HasForeignKey(x => x.CertificationSchemeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
