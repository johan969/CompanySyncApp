using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class BusinessRelationshipStageConfig : IEntityTypeConfiguration<BusinessRelationshipStage>
    {
        public void Configure(EntityTypeBuilder<BusinessRelationshipStage> builder)
        {
            builder.ToTable("BusinessRelationshipStages");

            builder.HasKey(x => x.Id);

            /*
             * Denna alternate key används av BusinessRelationship för att
             * säkerställa att valt steg tillhör rätt relationstyp.
             */
            builder.HasAlternateKey(x => new
            {
                x.BusinessRelationshipTypeId,
                x.Id
            });

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => new
            {
                x.BusinessRelationshipTypeId,
                x.Code
            })
            .IsUnique();

            builder.HasIndex(x => new
            {
                x.BusinessRelationshipTypeId,
                x.SortOrder
            });

            builder.HasOne(x => x.BusinessRelationshipType)
                .WithMany(x => x.Stages)
                .HasForeignKey(x => x.BusinessRelationshipTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}