using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class CertificationSchemeConfig : IEntityTypeConfiguration<CertificationScheme>
    {
        public void Configure(EntityTypeBuilder<CertificationScheme> builder)
        {
            builder.ToTable(
                "CertificationSchemes",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_CertificationSchemes_Validity",
                        """
                    "ValidToUtc" IS NULL
                    OR "ValidFromUtc" IS NULL
                    OR "ValidToUtc" >= "ValidFromUtc"
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.Kind)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.StandardOwner)
                .HasMaxLength(255);

            builder.Property(x => x.StandardVersion)
                .HasMaxLength(100);

            builder.Property(x => x.PublicUrl)
                .HasMaxLength(2048);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.Kind,
                x.IsActive
            });
        }
    }
}
