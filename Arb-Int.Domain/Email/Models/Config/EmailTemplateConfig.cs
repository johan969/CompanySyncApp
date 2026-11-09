using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Email.Configurations
{
    public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
    {
        public void Configure(EntityTypeBuilder<EmailTemplate> b)
        {
            b.ToTable("EmailTemplate");

            b.HasKey(x => x.Id);

            b.Property(x => x.Key)
                .HasMaxLength(256)
                .IsRequired();
            b.Property(x => x.Scope)
                .HasConversion<string>()
                .HasDefaultValue(EmailTemplateScope.Organisation)
                .IsRequired();
            b.Property(x => x.Kind)
                .HasConversion<string>()
                .HasDefaultValue(EmailTemplateKind.Generic)
                .IsRequired();

            // Scope/owner-regler (enforced i app-lager; check constraints om du vill)
            b.HasIndex(x => new { x.Scope, x.Key, x.OrganisationId, x.UserId })
             .IsUnique();

            b.HasMany(x => x.Variants)
                .WithOne(x => x.Template)
                .HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Bra att kunna söka på Key snabbt per scope
            b.HasIndex(x => new { x.Scope, x.Key });
        }
    }
}