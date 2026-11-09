using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar matchningsregelverk och immutable publicerade versioner.
/// </summary>
public interface IMatchingRuleSetService
{
    /// <summary>
    /// Hämtar ett regelverk med versionshistorik.
    /// </summary>
    Task<Result<MatchingRuleSetResponse>> GetByIdAsync(Guid matchingRuleSetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar regelverk och kan exkludera pensionerade poster.
    /// </summary>
    Task<Result<IReadOnlyList<MatchingRuleSetResponse>>> GetAllAsync(bool includeRetired = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett regelverk tillsammans med version 1.
    /// </summary>
    Task<Result<MatchingRuleSetResponse>> CreateAsync(CreateMatchingRuleSetRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar regelverkets metadata utan att ändra versionsinnehåll.
    /// </summary>
    Task<Result<MatchingRuleSetResponse>> UpdateAsync(Guid matchingRuleSetId, UpdateMatchingRuleSetRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar en opublicerad regelversion.
    /// </summary>
    Task<Result<MatchingRuleSetResponse>> UpdateVersionAsync(Guid matchingRuleSetVersionId, UpdateMatchingRuleSetVersionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Klonar senaste publicerade version till nästa opublicerade version.
    /// </summary>
    Task<Result<MatchingRuleSetResponse>> CreateNextVersionAsync(Guid matchingRuleSetId, CreateNextMatchingRuleSetVersionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publicerar en regelversion och aktiverar regelverket vid första publiceringen.
    /// </summary>
    Task<Result<MatchingRuleSetResponse>> PublishVersionAsync(Guid matchingRuleSetVersionId, PublishMatchingRuleSetVersionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pensionerar regelverket för nya matching runs.
    /// </summary>
    Task<Result<MatchingRuleSetResponse>> RetireAsync(Guid matchingRuleSetId, RetireMatchingRuleSetRequest request, CancellationToken cancellationToken = default);
}