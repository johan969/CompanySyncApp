using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Email.Configurations
{
    public class ProviderEmailTemplateMappingConfiguration : IEntityTypeConfiguration<ProviderEmailTemplateMapping>
    {
        public void Configure(EntityTypeBuilder<ProviderEmailTemplateMapping> b)
        {
            b.ToTable("ProviderEmailTemplateMapping");

            b.HasKey(x => x.Id);

            b.Property(x => x.ProviderTemplateId)
                .HasMaxLength(256)
                .IsRequired();

            b.HasOne(x => x.TemplateVariant)
                .WithMany(etv => etv.ProviderMappings)
                .HasForeignKey(x => x.TemplateVariantId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.TemplateVariantId, x.Provider })
                .IsUnique();

            b.HasIndex(x => new { x.Provider, x.ProviderTemplateId });
        }
    }
}