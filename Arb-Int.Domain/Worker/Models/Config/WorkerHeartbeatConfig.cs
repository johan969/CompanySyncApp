using Arb_Int.Domain.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Worker.Configurations
{
    public sealed class WorkerHeartbeatConfiguration : IEntityTypeConfiguration<WorkerHeartbeat>
    {
        public void Configure(EntityTypeBuilder<WorkerHeartbeat> b)
        {
            b.ToTable("WorkerHeartbeats");

            b.HasKey(x => x.InstanceId);

            b.Property(x => x.InstanceId)
                .HasMaxLength(128);

            b.Property(x => x.ServiceName)
                .HasMaxLength(128);

            b.Property(x => x.Version)
                .HasMaxLength(64);

            b.Property(x => x.Node)
                .HasMaxLength(128);

            b.Property(x => x.LastSeenUtc)
                .HasPrecision(0);

            b.Property(x => x.StartedUtc)
                .HasPrecision(0);

            b.HasIndex(x => x.LastSeenUtc);
            b.HasIndex(x => x.ServiceName);
        }
    }
}
