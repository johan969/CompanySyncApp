using Arb_Int.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(128);
        builder.Property(x => x.ReplacedByToken)
            .HasMaxLength(128);
        builder.Property(x => x.ReasonRevoked)
            .HasMaxLength(512);
        builder.Property(x => x.CreatedByIp)
            .HasMaxLength(45);
        builder.Property(x => x.RevokedByIp)
            .HasMaxLength(45);

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_RefreshTokens_UserId");
        builder.HasIndex(x => x.Token)
            .HasDatabaseName("IX_RefreshTokens_Token");

        builder.ToTable("RefreshToken");
    }
}