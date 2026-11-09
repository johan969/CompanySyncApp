using Arb_Int.Domain.Economy.Projections;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Arb_Int.Domain.Errors;

namespace Arb_Int.Domain.Economy;

/// <summary>
/// Läser valutor från referensdata.
/// </summary>
public sealed class CurrencyService : ICurrencyService
{
    private readonly Arb_IntDbContext _db;

    /// <summary>
    /// Skapar tjänsten med aktuell databascontext.
    /// </summary>
    public CurrencyService(Arb_IntDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Hämtar alla valutor i kodordning.
    /// </summary>
    public async Task<Result<IReadOnlyList<CurrencyResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _db.Set<Currency>()
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(CurrencyProjections.Response)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<CurrencyResponse>>(items);
    }

    /// <summary>
    /// Hämtar en valuta via id.
    /// </summary>
    public async Task<Result<CurrencyResponse>> GetByIdAsync(int currencyId, CancellationToken cancellationToken = default)
    {
        var item = await _db.Set<Currency>()
            .AsNoTracking()
            .Where(x => x.Id == currencyId)
            .Select(CurrencyProjections.Response)
            .SingleOrDefaultAsync(cancellationToken);

        return item is null
            ? Result.Fail<CurrencyResponse>(new NotFoundError("Currency was not found.", currencyId))
            : Result.Ok(item);
    }
}
