using Arb_Int.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Location.Configurations
{
    public class WaterConfig : IEntityTypeConfiguration<Water>
    {
        public void Configure(EntityTypeBuilder<Water> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.HasOne(w => w.ParentWater)
                .WithMany(w => w.ChildWaters)
                .HasForeignKey(w => w.ParentWaterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(w => w.ParentWaterId);
        }
    }
}