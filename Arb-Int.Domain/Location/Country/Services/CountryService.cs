using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Location.Projections;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Location;

/// <summary>
/// Läser länder från referensdata.
/// </summary>
public sealed class CountryService : ICountryService
{
    private readonly Arb_IntDbContext _db;

    /// <summary>
    /// Skapar tjänsten med aktuell databascontext.
    /// </summary>
    public CountryService(Arb_IntDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Söker och filtrerar länder.
    /// </summary>
    public async Task<Result<IReadOnlyList<CountryListResponse>>> QueryAsync(QueryCountriesRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<Country>().AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var pattern = $"%{request.Search.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.Name, pattern)
                || (x.Alpha2Code != null && EF.Functions.ILike(x.Alpha2Code, pattern))
                || (x.Alpha3Code != null && EF.Functions.ILike(x.Alpha3Code, pattern))
                || (x.ISO3166Numeric != null && EF.Functions.ILike(x.ISO3166Numeric, pattern)));
        }

        if (request.ContinentId.HasValue)
            query = query.Where(x => x.ContinentId == request.ContinentId.Value);

        if (request.IsEUCountry.HasValue)
            query = query.Where(x => x.IsEUCountry == request.IsEUCountry.Value);

        if (request.CurrencyId.HasValue)
            query = query.Where(x => x.CurrencyId == request.CurrencyId.Value);

        var items = await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(CountryProjections.List)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<CountryListResponse>>(items);
    }

    /// <summary>
    /// Hämtar ett land via id.
    /// </summary>
    public async Task<Result<CountryResponse>> GetByIdAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        var item = await _db.Set<Country>()
            .AsNoTracking()
            .Where(x => x.Id == countryId)
            .Select(CountryProjections.Response)
            .SingleOrDefaultAsync(cancellationToken);

        return item is null
            ? Result.Fail<CountryResponse>(new NotFoundError("Country was not found.", countryId))
            : Result.Ok(item);
    }

    /// <summary>
    /// Hämtar vattenområden som är kopplade till landet.
    /// </summary>
    public async Task<Result<IReadOnlyList<WaterListResponse>>> GetWatersAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        var exists = await _db.Set<Country>().AsNoTracking().AnyAsync(x => x.Id == countryId, cancellationToken);
        if (!exists)
            return Result.Fail<IReadOnlyList<WaterListResponse>>(new NotFoundError("Country was not found.", countryId));

        var items = await _db.Set<CountryWater>()
            .AsNoTracking()
            .Where(x => x.CountryId == countryId)
            .Select(x => x.Water)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(WaterProjections.List)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<WaterListResponse>>(items);
    }
}
