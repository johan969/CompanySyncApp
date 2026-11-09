using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Parties.Configurations
{
    public class OrganizationConfig : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.ToTable("Organizations", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Organizations_Parent_NotSelf",
                    "\"ParentOrganizationId\" IS NULL " +
                    "OR \"ParentOrganizationId\" <> \"Id\"");
            });

            builder.Property(x => x.LegalName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.NormalizedLegalName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.TradingName)
                .HasMaxLength(255);

            builder.Property(x => x.OrganizationKind)
                .HasConversion<string>()
                .HasMaxLength(64)
                .HasDefaultValue(OrganizationKind.Unknown);

            builder.Property(x => x.IsInternal)
                .HasDefaultValue(false);

            builder.HasIndex(x => x.NormalizedLegalName);

            builder.HasIndex(x => x.RegistrationCountryId);

            builder.HasOne(x => x.RegistrationCountry)
                .WithMany()
                .HasForeignKey(x => x.RegistrationCountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ParentOrganization)
                .WithMany(x => x.ChildOrganizations)
                .HasForeignKey(x => x.ParentOrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}