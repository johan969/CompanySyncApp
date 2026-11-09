using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar worker-genererade requirement evaluations och score contributions under en aktiv matching run.
/// </summary>
public interface IMatchingEvaluationExecutionService
{
    /// <summary>
    /// Skapar ett requirement-resultat för en candidate.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> AddRequirementEvaluationAsync(Guid supplySolutionCandidateId, CreateRequirementEvaluationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar resultatet för en befintlig requirement evaluation.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> UpdateRequirementEvaluationAsync(Guid requirementEvaluationId, UpdateRequirementEvaluationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort en requirement evaluation som ännu inte används av score contributions.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> DeleteRequirementEvaluationAsync(Guid requirementEvaluationId, uint rowVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lägger till ett score-bidrag för kandidaten.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> AddScoreContributionAsync(Guid supplySolutionCandidateId, CreateScoreContributionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett befintligt score-bidrag.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> UpdateScoreContributionAsync(Guid scoreContributionId, UpdateScoreContributionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort ett score-bidrag under pågående matching.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> DeleteScoreContributionAsync(Guid scoreContributionId, uint rowVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sparar regelmotorns sammanvägda outcome, verification och total score på kandidaten.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> SetCandidateEvaluationSummaryAsync(Guid supplySolutionCandidateId, SetCandidateEvaluationSummaryRequest request, CancellationToken cancellationToken = default);
}