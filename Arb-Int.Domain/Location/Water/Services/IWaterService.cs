using FluentResults;

namespace Arb_Int.Domain.Location;

/// <summary>
/// Läser hierarkiska vattenområden som referensdata.
/// </summary>
public interface IWaterService
{
    /// <summary>
    /// Söker och filtrerar vattenområden.
    /// </summary>
    Task<Result<IReadOnlyList<WaterListResponse>>> QueryAsync(QueryWatersRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar ett vattenområde inklusive direkta barn och kopplade länder.
    /// </summary>
    Task<Result<WaterResponse>> GetByIdAsync(Guid waterId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar direkt underordnade vattenområden.
    /// </summary>
    Task<Result<IReadOnlyList<WaterListResponse>>> GetChildrenAsync(Guid waterId, CancellationToken cancellationToken = default);
}
