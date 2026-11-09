using Arb_Int.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public sealed class ApplicationUserLoginConfiguration : IEntityTypeConfiguration<ApplicationUserLogin>
{
    public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder)
    {
        builder.HasKey(x => new { x.LoginProvider, x.ProviderKey });

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_AspNetUserLogins_UserId");
        builder.ToTable("UserLogin");
    }
}