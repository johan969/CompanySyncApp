
using Arb_Int.Domain;
using Arb_Int.Domain.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace Arb_Int.Repository.System
{
    public sealed class ScheduledTaskClaimer
    {
        private readonly Arb_IntDbContext _db;
        private readonly string _instanceId;

        public ScheduledTaskClaimer(Arb_IntDbContext db)
        {
            _db = db;
            _instanceId = Environment.MachineName + ":" + Guid.NewGuid().ToString("N")[..8];
        }

        /// <summary>
        /// Claimar förfallna ScheduledTasks och låser dem för den aktuella worker-instansen.
        /// </summary>
        public async Task<List<ScheduledTask>> ClaimDueAsync(
            int take,
            TimeSpan lease,
            CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;
            var lockedUntil = now.Add(lease);

            var pending = ScheduledTaskStatus.Pending.ToString();
            var dispatching = ScheduledTaskStatus.Dispatching.ToString();

            const string sql = """
        WITH candidates AS (
            SELECT st."Id", st."RunAtUtc"
            FROM public."ScheduledTasks" AS st
            WHERE st."Status" = @pending
              AND st."HangfireJobId" IS NULL
              AND st."RunAtUtc" <= @now
              AND (
                  st."LockedUntilUtc" IS NULL
                  OR st."LockedUntilUtc" < @now
              )
            ORDER BY st."RunAtUtc" ASC, st."Id" ASC
            LIMIT @take
            FOR UPDATE OF st SKIP LOCKED
        ),
        claimed_rows AS (
            UPDATE public."ScheduledTasks" AS st
            SET "Status" = @dispatching,
                "LockedUntilUtc" = @lockedUntil,
                "LockedBy" = @lockedBy
            FROM candidates AS c
            WHERE st."Id" = c."Id"
            RETURNING st."Id", st."RunAtUtc"
        )
        SELECT cr."Id", cr."RunAtUtc"
        FROM claimed_rows AS cr
        ORDER BY cr."RunAtUtc" ASC, cr."Id" ASC
        """;

            var keys = await _db.Database
                .SqlQueryRaw<ClaimedTaskKey>(
                    sql,
                    new NpgsqlParameter("@pending", pending),
                    new NpgsqlParameter("@dispatching", dispatching),
                    new NpgsqlParameter("@now", now),
                    new NpgsqlParameter("@lockedUntil", lockedUntil),
                    new NpgsqlParameter("@lockedBy", _instanceId),
                    new NpgsqlParameter("@take", take))
                .ToListAsync(cancellationToken);

            if (keys.Count == 0)
                return [];

            var ids = keys.Select(k => k.Id).ToList();

            var tasks = await _db.ScheduledTasks
                .AsNoTracking()
                .Where(t => ids.Contains(t.Id))
                .ToListAsync(cancellationToken);

            // Bevarar exakt samma ordning som claim-operationen returnerade.
            var order = keys
                .Select((k, i) => (k.Id, i))
                .ToDictionary(x => x.Id, x => x.i);

            tasks.Sort((a, b) => order[a.Id].CompareTo(order[b.Id]));

            return tasks;
        }

        private sealed record ClaimedTaskKey(Guid Id, DateTimeOffset RunAtUtc);


        /// <summary>
        /// Claimar ett större antal förfallna ScheduledTasks med hjälp av en temporär PostgreSQL-tabell.
        /// </summary>
        public async Task<List<ScheduledTask>> ClaimDueLargeAsync(
            int take,
            TimeSpan lease,
            CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;
            var lockedUntil = now.Add(lease);

            var pending = ScheduledTaskStatus.Pending.ToString();
            var dispatching = ScheduledTaskStatus.Dispatching.ToString();

            await _db.Database.OpenConnectionAsync(cancellationToken);
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // Temporär PostgreSQL-tabell som automatiskt tas bort när transaktionen committas.
            await _db.Database.ExecuteSqlRawAsync(
                """
        CREATE TEMP TABLE "Claimed" (
            "Id" uuid NOT NULL PRIMARY KEY,
            "RunAtUtc" timestamp with time zone NOT NULL,
            "Ord" integer NOT NULL
        ) ON COMMIT DROP
        """,
                cancellationToken);

            const string claimSql = """
        WITH candidates AS (
            SELECT st."Id", st."RunAtUtc"
            FROM public."ScheduledTasks" AS st
            WHERE st."Status" = @pending
              AND st."HangfireJobId" IS NULL
              AND st."RunAtUtc" <= @now
              AND (
                  st."LockedUntilUtc" IS NULL
                  OR st."LockedUntilUtc" < @now
              )
            ORDER BY st."RunAtUtc" ASC, st."Id" ASC
            LIMIT @take
            FOR UPDATE OF st SKIP LOCKED
        ),
        ordered_candidates AS (
            SELECT
                c."Id",
                c."RunAtUtc",
                CAST(
                    ROW_NUMBER() OVER (
                        ORDER BY c."RunAtUtc" ASC, c."Id" ASC
                    )
                    AS integer
                ) AS "Ord"
            FROM candidates AS c
        ),
        updated_rows AS (
            UPDATE public."ScheduledTasks" AS st
            SET "Status" = @dispatching,
                "LockedUntilUtc" = @lockedUntil,
                "LockedBy" = @lockedBy
            FROM ordered_candidates AS c
            WHERE st."Id" = c."Id"
            RETURNING st."Id", st."RunAtUtc", c."Ord"
        )
        INSERT INTO "Claimed" ("Id", "RunAtUtc", "Ord")
        SELECT ur."Id", ur."RunAtUtc", ur."Ord"
        FROM updated_rows AS ur
        """;

            await _db.Database.ExecuteSqlRawAsync(
                claimSql,
                new NpgsqlParameter("@pending", pending),
                new NpgsqlParameter("@dispatching", dispatching),
                new NpgsqlParameter("@now", now),
                new NpgsqlParameter("@lockedUntil", lockedUntil),
                new NpgsqlParameter("@lockedBy", _instanceId),
                new NpgsqlParameter("@take", take));

            var idsOrdered = new List<Guid>();

            await using (var cmd = _db.Database.GetDbConnection().CreateCommand())
            {
                cmd.Transaction = tx.GetDbTransaction();
                cmd.CommandText = """SELECT "Id" FROM "Claimed" ORDER BY "Ord" ASC""";

                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync(cancellationToken))
                    idsOrdered.Add(reader.GetGuid(0));
            }

            if (idsOrdered.Count == 0)
            {
                await tx.CommitAsync(cancellationToken);
                return [];
            }

            var result = new List<ScheduledTask>(idsOrdered.Count);
            const int chunkSize = 1000;

            for (var i = 0; i < idsOrdered.Count; i += chunkSize)
            {
                var chunk = idsOrdered
                    .Skip(i)
                    .Take(chunkSize)
                    .ToList();

                var chunkTasks = await _db.ScheduledTasks
                    .AsNoTracking()
                    .Where(t => chunk.Contains(t.Id))
                    .ToListAsync(cancellationToken);

                var dict = chunkTasks.ToDictionary(t => t.Id);

                foreach (var id in chunk)
                {
                    if (dict.TryGetValue(id, out var task))
                        result.Add(task);
                }
            }

            await tx.CommitAsync(cancellationToken);

            return result;
        }

    }
}
