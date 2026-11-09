using Arb_Int.Domain;
using Arb_Int.Domain.Worker;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.WorkerService
{
    public sealed class ScheduledTaskDefinitionClaimer
    {
        private readonly Arb_IntDbContext _db;
        private readonly string _instanceId;

        public ScheduledTaskDefinitionClaimer(Arb_IntDbContext db)
        {
            _db = db;
            _instanceId = Environment.MachineName + ":" + Guid.NewGuid().ToString("N")[..8];
        }

        /// <summary>
        /// Claimar förfallna ScheduledTaskDefinitions och låser dem för den aktuella worker-instansen.
        /// </summary>
        public async Task<List<Guid>> ClaimDueAsync(
            int take,
            TimeSpan lease,
            CancellationToken cancellationToken = default)
        {
            var nowUtc = DateTimeOffset.UtcNow;
            var lockedUntilUtc = nowUtc.Add(lease);

            var enabled = ScheduledTaskDefinitionStatus.Enabled.ToString();

            const string sql = """
        WITH candidates AS (
            SELECT d."Id", d."NextRunAtUtc"
            FROM public."ScheduledTaskDefinitions" AS d
            WHERE d."Status" = @enabled
              AND d."NextRunAtUtc" <= @nowUtc
              AND (
                  d."EndAtUtc" IS NULL
                  OR d."EndAtUtc" >= d."NextRunAtUtc"
              )
              AND (
                  d."LockedUntilUtc" IS NULL
                  OR d."LockedUntilUtc" < @nowUtc
              )
            ORDER BY d."NextRunAtUtc" ASC, d."Id" ASC
            LIMIT @take
            FOR UPDATE OF d SKIP LOCKED
        ),
        claimed_rows AS (
            UPDATE public."ScheduledTaskDefinitions" AS d
            SET "LockedUntilUtc" = @lockedUntilUtc,
                "LockedBy" = @lockedBy
            FROM candidates AS c
            WHERE d."Id" = c."Id"
            RETURNING d."Id", c."NextRunAtUtc"
        )
        SELECT cr."Id"
        FROM claimed_rows AS cr
        ORDER BY cr."NextRunAtUtc" ASC, cr."Id" ASC
        """;

            var ids = await _db.Database
                .SqlQueryRaw<Guid>(
                    sql,
                    new NpgsqlParameter("@enabled", enabled),
                    new NpgsqlParameter("@nowUtc", nowUtc),
                    new NpgsqlParameter("@lockedUntilUtc", lockedUntilUtc),
                    new NpgsqlParameter("@lockedBy", _instanceId),
                    new NpgsqlParameter("@take", take))
                .ToListAsync(cancellationToken);

            // Raw SQL kringgår EF:s ChangeTracker, så eventuellt gammalt tracked state tas bort.
            _db.ChangeTracker.Clear();

            return ids;
        }
    }
}
