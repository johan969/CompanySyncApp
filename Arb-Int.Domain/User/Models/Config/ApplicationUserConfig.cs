using Arb_Int.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(e => e.IsPasswordChangeRequired).HasDefaultValue(true);

        builder.HasIndex(x => x.NormalizedUserName)
            .HasDatabaseName("UserNameIndex")
            .IsUnique()
            .HasFilter("\"NormalizedUserName\" IS NOT NULL");

        builder.HasIndex(x => x.NormalizedEmail)
            .HasDatabaseName("EmailIndex");

        // Om du ofta slår upp användare via e-post vid login/import
        builder.HasIndex(x => x.Email)
            .HasDatabaseName("IX_AspNetUsers_Email");

        // Each User can have many UserClaims
        builder.HasMany(e => e.Claims)
            .WithOne(c => c.User)
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();

        // Each User can have many UserTokens
        builder.HasMany(e => e.Tokens)
            .WithOne(t => t.User)
            .HasForeignKey(ut => ut.UserId)
            .IsRequired();

        // Each User can have many UserLogins
        builder.HasMany(e => e.Logins)
            .WithOne(l => l.User)
            .HasForeignKey(ul => ul.UserId)
            .IsRequired();

        // Each User can have many UserRoles
        builder.HasMany(e => e.UserRoles)
            .WithOne(r => r.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        // Each User can have many Refresh tokens
        builder.HasMany(e => e.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(urt => urt.UserId)
            .IsRequired();

        builder.ToTable("User");

        // Om du ofta söker aktiva/inaktiva/verifierade användare,
        // lägg bara till om propertyn finns på ApplicationUser.
        // builder.HasIndex(x => x.EmailConfirmed)
        //     .HasDatabaseName("IX_AspNetUsers_EmailConfirmed");
    }
}