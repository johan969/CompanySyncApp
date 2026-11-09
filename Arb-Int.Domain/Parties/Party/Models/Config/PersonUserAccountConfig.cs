using Arb_Int.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Parties.Configurations
{
    /// <summary>
    /// Konfigurerar PersonUserAccount och dess 1-1-kopplingar till Person och ApplicationUser.
    /// </summary>
    public class PersonUserAccountConfig : IEntityTypeConfiguration<PersonUserAccount>
    {
        /// <summary>
        /// Konfigurerar tabell, concurrency, index och foreign keys.
        /// </summary>
        public void Configure(EntityTypeBuilder<PersonUserAccount> builder)
        {
            builder.ToTable("PersonUserAccounts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName).HasMaxLength(255);
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(64);
            builder.Property(x => x.Notes).HasMaxLength(4096);

            builder.HasIndex(x => x.PersonId).IsUnique();
            builder.HasIndex(x => x.ApplicationUserId).IsUnique();
            builder.HasIndex(x => x.AuthEmailContactPointId);
            builder.HasIndex(x => new { x.IsExternalUser, x.Status });

            builder.HasOne(x => x.Person)
                .WithOne(x => x.UserAccount)
                .HasForeignKey<PersonUserAccount>(x => x.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ApplicationUser)
                .WithOne(x => x.PersonAccount)
                .HasForeignKey<PersonUserAccount>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AuthEmailContactPoint)
                .WithMany()
                .HasForeignKey(x => x.AuthEmailContactPointId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
