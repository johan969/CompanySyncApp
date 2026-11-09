using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Email.Configurations
{
    public class EmailTemplateVariantConfiguration : IEntityTypeConfiguration<EmailTemplateVariant>
    {
        public void Configure(EntityTypeBuilder<EmailTemplateVariant> b)
        {
            b.ToTable("EmailTemplateVariant");

            b.HasKey(x => x.Id);

            b.Property(x => x.Culture)
                .HasMaxLength(32)
                .IsRequired();
            b.Property(x => x.SubjectTemplate)
                .HasMaxLength(512)
                .IsRequired();
            b.Property(x => x.BodyHtmlTemplate)
                .HasMaxLength(65536)
                .IsRequired();

            // en variant per culture per template (om du inte versionshanterar)
            b.HasIndex(x => new { x.TemplateId, x.Culture })
                .IsUnique();

            // valfritt: endast en default per template
            b.HasIndex(x => new { x.TemplateId, x.IsDefault })
                .HasFilter("\"IsDefault\" = true")
                .IsUnique();
        }
    }
}