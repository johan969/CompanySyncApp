using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Parties.Configurations
{
    public class AddressConfig : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Addresses_HasAddressData",
                    """
                    "PostOfficeBox" IS NOT NULL
                    OR "AddressLine1" IS NOT NULL
                    OR "AddressLine2" IS NOT NULL
                    OR "AddressLine3" IS NOT NULL
                    OR "PostalCode" IS NOT NULL
                    OR "Locality" IS NOT NULL
                    OR "AdministrativeArea" IS NOT NULL
                    """);
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CareOf)
                .HasMaxLength(255);

            builder.Property(x => x.PostOfficeBox)
                .HasMaxLength(100);

            builder.Property(x => x.AddressLine1)
                .HasMaxLength(255);

            builder.Property(x => x.AddressLine2)
                .HasMaxLength(255);

            builder.Property(x => x.AddressLine3)
                .HasMaxLength(255);

            builder.Property(x => x.PostalCode)
                .HasMaxLength(50);

            builder.Property(x => x.Locality)
                .HasMaxLength(255);

            builder.Property(x => x.AdministrativeArea)
                .HasMaxLength(255);

            builder.Property(x => x.AddressKey)
                .IsRequired()
                .HasMaxLength(128);

            builder.HasIndex(x => x.AddressKey)
                .IsUnique();

            builder.HasIndex(x => x.CountryId);

            builder.HasOne(x => x.Country)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}