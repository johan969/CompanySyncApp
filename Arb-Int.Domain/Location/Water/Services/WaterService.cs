using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Location.Projections;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Location;

/// <summary>
/// Läser vattenområden från referensdata.
/// </summary>
public sealed class WaterService : IWaterService
{
    private readonly Arb_IntDbContext _db;

    /// <summary>
    /// Skapar tjänsten med aktuell databascontext.
    /// </summary>
    public WaterService(Arb_IntDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Söker och filtrerar vattenområden.
    /// </summary>
    public async Task<Result<IReadOnlyList<WaterListResponse>>> QueryAsync(QueryWatersRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<Water>().AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var pattern = $"%{request.Search.Trim()}%";
            query = query.Where(x => EF.Functions.ILike(x.Name, pattern) || EF.Functions.ILike(x.Description, pattern));
        }

        if (request.Type.HasValue)
            query = query.Where(x => x.Type == request.Type.Value);

        if (request.FeatureType.HasValue)
            query = query.Where(x => x.FeatureType == request.FeatureType.Value);

        var items = await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(WaterProjections.List)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<WaterListResponse>>(items);
    }

    /// <summary>
    /// Hämtar ett vattenområde inklusive direkta barn och kopplade länder.
    /// </summary>
    public async Task<Result<WaterResponse>> GetByIdAsync(Guid waterId, CancellationToken cancellationToken = default)
    {
        var item = await _db.Set<Water>()
            .AsNoTracking()
            .Where(x => x.Id == waterId)
            .Select(x => new WaterResponse
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                FeatureType = x.FeatureType,
                Description = x.Description,
                ParentWaterId = x.ParentWaterId,
                ParentWaterName = x.ParentWater != null ? x.ParentWater.Name : null,
                Children = x.ChildWaters
                    .OrderBy(child => child.Name)
                    .ThenBy(child => child.Id)
                    .Select(child => new WaterListResponse
                    {
                        Id = child.Id,
                        Name = child.Name,
                        Type = child.Type,
                        FeatureType = child.FeatureType,
                        ParentWaterId = child.ParentWaterId,
                        ParentWaterName = x.Name
                    })
                    .ToList(),
                Countries = x.CountryWaters
                    .Select(link => link.Country)
                    .OrderBy(country => country.Name)
                    .ThenBy(country => country.Id)
                    .Select(country => new CountryListResponse
                    {
                        Id = country.Id,
                        Name = country.Name,
                        IsEUCountry = country.IsEUCountry,
                        Alpha2Code = country.Alpha2Code,
                        Alpha3Code = country.Alpha3Code,
                        ContinentId = country.ContinentId,
                        ContinentName = country.Continent.Name,
                        CurrencyId = country.CurrencyId,
                        CurrencyCode = country.Currency != null ? country.Currency.Code : null
                    })
                    .ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);

        return item is null
            ? Result.Fail<WaterResponse>(new NotFoundError("Water was not found.", waterId))
            : Result.Ok(item);
    }

    /// <summary>
    /// Hämtar direkt underordnade vattenområden.
    /// </summary>
    public async Task<Result<IReadOnlyList<WaterListResponse>>> GetChildrenAsync(Guid waterId, CancellationToken cancellationToken = default)
    {
        var exists = await _db.Set<Water>().AsNoTracking().AnyAsync(x => x.Id == waterId, cancellationToken);
        if (!exists)
            return Result.Fail<IReadOnlyList<WaterListResponse>>(new NotFoundError("Water was not found.", waterId));

        var items = await _db.Set<Water>()
            .AsNoTracking()
            .Where(x => x.ParentWaterId == waterId)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(WaterProjections.List)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<WaterListResponse>>(items);
    }
}
