using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    /// <summary>
    /// Konfigurerar versionshistoriken för ett matchningsregelverk.
    /// </summary>
    public sealed class MatchingRuleSetVersionConfig : IEntityTypeConfiguration<MatchingRuleSetVersion>
    {
        /// <summary>
        /// Konfigurerar tabell, constraints, index och relationer för regelversioner.
        /// </summary>
        public void Configure(EntityTypeBuilder<MatchingRuleSetVersion> builder)
        {
            builder.ToTable(
                "MatchingRuleSetVersions",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_MatchingRuleSetVersions_VersionNumber",
                        """
                    "VersionNumber" > 0
                    """);

                    table.HasCheckConstraint(
                        "CK_MatchingRuleSetVersions_EffectiveWindow",
                        """
                    "EffectiveToUtc" IS NULL
                    OR "EffectiveFromUtc" IS NULL
                    OR "EffectiveToUtc" >= "EffectiveFromUtc"
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName).HasMaxLength(255);
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.Description).HasMaxLength(4096);
            builder.Property(x => x.ConfigurationJson).HasColumnType("jsonb").IsRequired();
            builder.Property(x => x.ConfigurationHash).HasMaxLength(128).IsRequired();

            builder.HasIndex(x => new { x.MatchingRuleSetId, x.VersionNumber }).IsUnique();

            /*
             * Bara en redigerbar version får finnas samtidigt.
             * Publicerade versioner påverkas inte av indexet.
             */
            builder.HasIndex(x => x.MatchingRuleSetId)
                .IsUnique()
                .HasFilter("\"PublishedAtUtc\" IS NULL")
                .HasDatabaseName("UX_MatchingRuleSetVersions_OneUnpublished");

            builder.HasIndex(x => x.ConfigurationHash);
        }
    }
}
