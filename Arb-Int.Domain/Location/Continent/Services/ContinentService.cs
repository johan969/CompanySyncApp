using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Location.Projections;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Location;

/// <summary>
/// Läser kontinenter från referensdata.
/// </summary>
public sealed class ContinentService : IContinentService
{
    private readonly Arb_IntDbContext _db;

    /// <summary>
    /// Skapar tjänsten med aktuell databascontext.
    /// </summary>
    public ContinentService(Arb_IntDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Hämtar alla kontinenter i namnordning.
    /// </summary>
    public async Task<Result<IReadOnlyList<ContinentResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _db.Set<Continent>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(ContinentProjections.Response)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<ContinentResponse>>(items);
    }

    /// <summary>
    /// Hämtar en kontinent via id.
    /// </summary>
    public async Task<Result<ContinentResponse>> GetByIdAsync(int continentId, CancellationToken cancellationToken = default)
    {
        var item = await _db.Set<Continent>()
            .AsNoTracking()
            .Where(x => x.Id == continentId)
            .Select(ContinentProjections.Response)
            .SingleOrDefaultAsync(cancellationToken);

        return item is null
            ? Result.Fail<ContinentResponse>(new NotFoundError("Continent was not found.", continentId))
            : Result.Ok(item);
    }

    /// <summary>
    /// Hämtar alla länder på en kontinent i namnordning.
    /// </summary>
    public async Task<Result<IReadOnlyList<CountryListResponse>>> GetCountriesAsync(int continentId, CancellationToken cancellationToken = default)
    {
        var exists = await _db.Set<Continent>().AsNoTracking().AnyAsync(x => x.Id == continentId, cancellationToken);
        if (!exists)
            return Result.Fail<IReadOnlyList<CountryListResponse>>(new NotFoundError("Continent was not found.", continentId));

        var items = await _db.Set<Country>()
            .AsNoTracking()
            .Where(x => x.ContinentId == continentId)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(CountryProjections.List)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<CountryListResponse>>(items);
    }
}
