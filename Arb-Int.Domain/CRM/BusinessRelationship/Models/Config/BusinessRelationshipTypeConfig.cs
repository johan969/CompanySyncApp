using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class BusinessRelationshipTypeConfig : IEntityTypeConfiguration<BusinessRelationshipType>
    {
        public void Configure(EntityTypeBuilder<BusinessRelationshipType> builder)
        {
            builder.ToTable("BusinessRelationshipTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(x => x.Code)
                .IsUnique();
        }
    }
}