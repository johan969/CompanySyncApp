using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class BusinessRelationshipContactConfig : IEntityTypeConfiguration<BusinessRelationshipContact>
    {
        public void Configure(EntityTypeBuilder<BusinessRelationshipContact> builder)
        {
            builder.ToTable("BusinessRelationshipContacts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(
                    BusinessRelationshipContactRole.Unknown);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.BusinessRelationshipId,
                x.OrganizationPersonId,
                x.Role
            })
            .IsUnique();

            builder.HasIndex(x => x.BusinessRelationshipId)
                .IsUnique()
                .HasFilter("\"IsPrimary\" = TRUE");

            builder.HasIndex(x => x.OrganizationPersonId);

            builder.HasOne(x => x.BusinessRelationship)
                .WithMany(x => x.Contacts)
                .HasForeignKey(x => x.BusinessRelationshipId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.OrganizationPerson)
                .WithMany()
                .HasForeignKey(x => x.OrganizationPersonId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}