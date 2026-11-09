using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Parties.Configurations
{
    public class PartyAddressConfig :
        IEntityTypeConfiguration<PartyAddress>
    {
        public void Configure(EntityTypeBuilder<PartyAddress> builder)
        {
            builder.ToTable("PartyAddresses", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_PartyAddresses_ValidDates",
                    "\"ValidToUtc\" IS NULL " +
                    "OR \"ValidToUtc\" >= \"ValidFromUtc\"");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UsageType)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(AddressUsageType.Unknown);

            builder.Property(x => x.Label)
                .HasMaxLength(255);

            builder.Property(x => x.Attention)
                .HasMaxLength(255);

            builder.HasIndex(x => new
            {
                x.PartyId,
                x.AddressId,
                x.UsageType
            })
            .IsUnique()
            .HasFilter("\"ValidToUtc\" IS NULL");

            builder.HasIndex(x => new
            {
                x.PartyId,
                x.UsageType
            })
            .IsUnique()
            .HasFilter(
                "\"IsPrimary\" = TRUE " +
                "AND \"ValidToUtc\" IS NULL");

            builder.HasIndex(x => x.AddressId);

            builder.HasOne(x => x.Party)
                .WithMany(x => x.AddressLinks)
                .HasForeignKey(x => x.PartyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Address)
                .WithMany(x => x.PartyLinks)
                .HasForeignKey(x => x.AddressId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}