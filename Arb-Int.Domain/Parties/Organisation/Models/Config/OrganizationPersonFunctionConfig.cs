using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Parties.Configurations
{
    public class OrganizationPersonFunctionConfig :
        IEntityTypeConfiguration<OrganizationPersonFunction>
    {
        public void Configure(
            EntityTypeBuilder<OrganizationPersonFunction> builder)
        {
            builder.ToTable("OrganizationPersonFunctions");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.OrganizationPersonId,
                x.ContactFunctionTypeId
            })
            .IsUnique();

            builder.HasIndex(x => x.ContactFunctionTypeId);

            builder.HasOne(x => x.OrganizationPerson)
                .WithMany(x => x.FunctionLinks)
                .HasForeignKey(x => x.OrganizationPersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ContactFunctionType)
                .WithMany(x => x.OrganizationPersonLinks)
                .HasForeignKey(x => x.ContactFunctionTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}