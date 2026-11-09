using FluentResults;

namespace Arb_Int.Domain.Economy;

/// <summary>
/// Läser valutor som referensdata.
/// </summary>
public interface ICurrencyService
{
    /// <summary>
    /// Hämtar alla valutor i kodordning.
    /// </summary>
    Task<Result<IReadOnlyList<CurrencyResponse>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar en valuta via id.
    /// </summary>
    Task<Result<CurrencyResponse>> GetByIdAsync(int currencyId, CancellationToken cancellationToken = default);
}
