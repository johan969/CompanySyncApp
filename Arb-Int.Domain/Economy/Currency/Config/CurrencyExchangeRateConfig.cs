using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Economy.Configurations
{
    public class CurrencyExchangeRateChangeConfig : IEntityTypeConfiguration<CurrencyExchangeRateChange>
    {
        public void Configure(EntityTypeBuilder<CurrencyExchangeRateChange> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Source).HasMaxLength(128);
            builder.Property(x => x.ExchangeRate).HasPrecision(19, 8);
            builder.Property(x => x.PreviousExchangeRate).HasPrecision(19, 8);
            builder
                .HasIndex(x => new { x.CurrencyId, x.RateDate })
                .IsUnique();
        }
    }
}