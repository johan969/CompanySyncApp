using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Parties.Configurations
{
    public class ContactFunctionTypeConfig :
        IEntityTypeConfiguration<ContactFunctionType>
    {
        public void Configure(
            EntityTypeBuilder<ContactFunctionType> builder)
        {
            builder.ToTable("ContactFunctionTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Code)
                .IsUnique();
        }
    }
}