using FluentResults;

namespace Arb_Int.Domain.Location;

/// <summary>
/// Läser kontinenter och deras länder som referensdata.
/// </summary>
public interface IContinentService
{
    /// <summary>
    /// Hämtar alla kontinenter i namnordning.
    /// </summary>
    Task<Result<IReadOnlyList<ContinentResponse>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar en kontinent via id.
    /// </summary>
    Task<Result<ContinentResponse>> GetByIdAsync(int continentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar alla länder på en kontinent i namnordning.
    /// </summary>
    Task<Result<IReadOnlyList<CountryListResponse>>> GetCountriesAsync(int continentId, CancellationToken cancellationToken = default);
}
