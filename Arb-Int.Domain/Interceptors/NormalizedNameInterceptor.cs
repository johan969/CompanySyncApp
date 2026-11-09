using Arb_Int.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public sealed class NormalizedNameInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        SetNormalizedNames(eventData.Context);

        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetNormalizedNames(eventData.Context);

        return ValueTask.FromResult(result);
    }

    private static void SetNormalizedNames(DbContext? dbContext)
    {
        if (dbContext is null)
        {
            return;
        }

        var entries = dbContext.ChangeTracker
            .Entries<INormalizedName>()
            .Where(entry =>
                entry.State == EntityState.Added ||
                entry.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Räkna alltid om NormalizedName när entiteten ska sparas så att värdet aldrig kan bli osynkat med Name.
            entry.Entity.NormalizedName =
                NameNormalizer.Normalize(entry.Entity.Name);
        }

        var legalEntries = dbContext.ChangeTracker
            .Entries<INormalizedLegalName>()
            .Where(entry =>
                entry.State == EntityState.Added ||
                entry.State == EntityState.Modified);

        foreach (var entry in legalEntries)
        {
            // Räkna alltid om NormalizedLegalName när entiteten ska sparas så att värdet aldrig kan bli osynkat med LegalName.
            entry.Entity.NormalizedLegalName =
                NameNormalizer.Normalize(entry.Entity.LegalName);
        }
    }
}