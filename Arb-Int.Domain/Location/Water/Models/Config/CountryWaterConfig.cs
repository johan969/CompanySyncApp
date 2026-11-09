using Arb_Int.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Location.Configurations
{
    public class CountryWaterConfig : IEntityTypeConfiguration<CountryWater>
    {
        public void Configure(EntityTypeBuilder<CountryWater> builder)
        {
            builder.HasKey(cw => new
            {
                cw.CountryId,
                cw.WaterId
            });

            builder.HasOne(cw => cw.Country)
                .WithMany(c => c.CountryWaters)
                .HasForeignKey(cw => cw.CountryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cw => cw.Water)
                .WithMany(w => w.CountryWaters)
                .HasForeignKey(cw => cw.WaterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(cw => cw.WaterId);
        }
    }
}