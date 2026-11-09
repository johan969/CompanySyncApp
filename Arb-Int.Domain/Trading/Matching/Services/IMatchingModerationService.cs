using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar manuell review och append-only moderation ovanpå immutable matching-resultat.
/// </summary>
public interface IMatchingModerationService
{
    /// <summary>
    /// Hämtar kandidatens moderation-historik med senaste beslut först.
    /// </summary>
    Task<Result<IReadOnlyList<MatchingModerationDecisionResponse>>> GetHistoryAsync(Guid supplySolutionCandidateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Flyttar en finaliserad Generated candidate till UnderReview.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> BeginReviewAsync(Guid supplySolutionCandidateId, BeginSupplySolutionCandidateReviewRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sparar ett append-only moderation-beslut och utför motsvarande explicita statusövergång.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> DecideAsync(Guid supplySolutionCandidateId, CreateMatchingModerationDecisionRequest request, CancellationToken cancellationToken = default);
}