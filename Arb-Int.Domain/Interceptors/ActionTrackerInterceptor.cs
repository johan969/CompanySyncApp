using Arb_Int.Domain.Base;
using Arb_Int.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Arb_Int.Domain
{
    public class ActionTrackerInterceptor : SaveChangesInterceptor
    {
        private static readonly Guid SystemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private readonly ICurrentIdentityService _currentIdentityService;
        public ActionTrackerInterceptor(ICurrentIdentityService currentIdentityService)
        {
            _currentIdentityService = currentIdentityService;
        }
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyActionTracking(eventData.Context);
            return base.SavingChanges(eventData, result);
        }
        private void ApplyActionTracking(DbContext? context)
        {
            if (context is null)
                return;

            var userId = _currentIdentityService.UserId ?? SystemUserId;
            var now = DateTimeOffset.UtcNow;

            foreach (var guidEntity in context.ChangeTracker.Entries<IEntity<Guid>>())
            {
                // Generera Guid-v7 på nya objekt om det är ett tomt Guid
                if (guidEntity is { Entity: IEntity<Guid> entity } && guidEntity.State == EntityState.Added && (entity.Id == Guid.Empty || entity.Id.Version != 7))
                    entity.Id = Guid.CreateVersion7();
            }
            foreach (var latestActivityTracker in context.ChangeTracker.Entries<ILatestActivityTracker>())
            {
                // Sätter LatestActivity på objekt som implementerar ILatestActivityTracker som skapas eller ändras
                if (latestActivityTracker is { Entity: ILatestActivityTracker latestActivity } &&
                    ((latestActivityTracker.State == EntityState.Added && !latestActivity.LatestActivityUtc.HasValue) ||
                        latestActivityTracker.State == EntityState.Modified))
                    latestActivity.LatestActivityUtc = now;
            }
            foreach (var createTracker in context.ChangeTracker.Entries<ICreateTracker>())
            {

                // Sätter CreateDate om entiteten skaps
                if (createTracker.State == EntityState.Added)
                {
                    createTracker.Entity.CreateDateUtc ??= now;
                }
            }
            foreach (var actionTrackerEntry in context.ChangeTracker.Entries<IActionTracker>())
            {
                // Sätter senaste action och vem som gjort den
                if (actionTrackerEntry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                {
                    actionTrackerEntry.Entity.ActionDateUtc ??= now;
                    actionTrackerEntry.Entity.ActionName ??= actionTrackerEntry.State.ToString();
                    actionTrackerEntry.Entity.ActionUserId = userId;
                }
            }
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var syncResult = SavingChanges(eventData, result);
            return ValueTask.FromResult(syncResult);
        }
    }
}