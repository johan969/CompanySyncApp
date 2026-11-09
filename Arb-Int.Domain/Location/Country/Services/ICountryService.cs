using FluentResults;

namespace Arb_Int.Domain.Location;

/// <summary>
/// Läser länder och landets vattenområden som referensdata.
/// </summary>
public interface ICountryService
{
    /// <summary>
    /// Söker och filtrerar länder.
    /// </summary>
    Task<Result<IReadOnlyList<CountryListResponse>>> QueryAsync(QueryCountriesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar ett land via id.
    /// </summary>
    Task<Result<CountryResponse>> GetByIdAsync(Guid countryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar vattenområden som är kopplade till landet.
    /// </summary>
    Task<Result<IReadOnlyList<WaterListResponse>>> GetWatersAsync(Guid countryId, CancellationToken cancellationToken = default);
}
